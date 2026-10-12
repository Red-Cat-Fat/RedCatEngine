using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;

namespace RedCatEngine.CommonServices.Services.Logs.Sinks
{
	/// <summary>
	/// Потокобезопасная запись логов в файл. Вызывается с любых потоков (Wwise, Mirror, Job System),
	/// поэтому внутри Write запрещены Unity API — только DateTime.UtcNow.
	/// Запись буферизована, фоновый поток сбрасывает её на диск; на Error сброс синхронный,
	/// чтобы запись пережила жёсткое завершение процесса.
	/// </summary>
	public sealed class FileLogSink : ILogSink, ILogFilePathProvider, IDisposable
	{
		private const int WriterBufferBytes = 65536;
		private const int WorkerWaitMilliseconds = 250;

		/// <summary>Минимальный интервал между синхронными сбросами на ошибке.</summary>
		private const long ErrorFlushIntervalTicks = TimeSpan.TicksPerSecond / 2;

		private const long MaxFileBytes = 32L * 1024 * 1024;
		private const long MaxTotalBytes = 512L * 1024 * 1024;
		private const int MaxSessionFiles = 10;
		private readonly string _filePrefix;

		private readonly string _folderPath;

		private readonly ConcurrentQueue<string> _queue = new();
		private readonly List<string> _sessionFilePaths = new();
		private readonly DateTime _sessionTime;
		private readonly AutoResetEvent _signal = new(false);

		private readonly Thread _worker;
		private readonly object _writerLock = new();
		private volatile bool _isDisposed;
		private volatile bool _isRunning = true;
		private long _lastErrorFlushTicks;
		private int _partIndex;

		private StreamWriter _writer;
		private long _writtenBytes;

		public FileLogSink(string folderName, string filePrefix)
		{
			_folderPath = LogFileLocation.ResolveRootFolder(folderName);
			_filePrefix = filePrefix;
			_sessionTime = DateTime.Now;

			Directory.CreateDirectory(_folderPath);
			CleanupOldFiles();
			OpenNextFile();

			_worker = new Thread(WorkerLoop)
			{
					Name = "FileLogSink",
					IsBackground = true
			};
			_worker.Start();
		}

		public bool IsEnabled { get; set; } = true;

		public void Dispose()
		{
			if(_isDisposed)
				return;

			_isDisposed = true;
			_isRunning = false;
			_signal.Set();

			if(_worker != null && _worker.IsAlive)
				_worker.Join(TimeSpan.FromMilliseconds(500));

			lock(_writerLock)
			{
				DrainQueueLocked();
				CloseWriterLocked();
			}

			_signal.Dispose();
		}

		public string CurrentLogFilePath { get; private set; }

		public IReadOnlyList<string> SessionFilePaths
		{
			get
			{
				lock(_writerLock)
					return _sessionFilePaths.ToArray();
			}
		}

		public void Write(in LogEntry entry)
		{
			if(!IsEnabled || _isDisposed)
				return;

			_queue.Enqueue(FormatLine(in entry));

			if(!string.IsNullOrEmpty(entry.StackTrace))
				_queue.Enqueue(FormatStackTrace(entry.StackTrace));

			if(entry.Level >= LogLevel.Error && TryTakeErrorFlushSlot())
			{
				// Ошибка может быть последним, что успеет случиться до падения процесса.
				// Но LogError в Update не должен превращаться в поток синхронных syscall.
				Flush();
				return;
			}

			_signal.Set();
		}

		public void Flush()
		{
			if(_isDisposed)
				return;

			lock(_writerLock)
			{
				DrainQueueLocked();

				try
				{
					_writer?.Flush();
				}
				catch(ObjectDisposedException)
				{
					// Гонка shutdown/dispose
				}
				catch(IOException)
				{
				}
			}
		}

		private bool TryTakeErrorFlushSlot()
		{
			var now = DateTime.UtcNow.Ticks;
			var last = Interlocked.Read(ref _lastErrorFlushTicks);

			if(now - last < ErrorFlushIntervalTicks)
				return false;

			return Interlocked.CompareExchange(ref _lastErrorFlushTicks, now, last) == last;
		}

		private void WorkerLoop()
		{
			while(_isRunning)
			{
				_signal.WaitOne(WorkerWaitMilliseconds);

				try
				{
					lock(_writerLock)
					{
						if(!DrainQueueLocked())
							continue;

						_writer?.Flush();
					}
				}
				catch(Exception)
				{
					// Логировать нечем — логгер и есть падающий компонент.
					// Главное не дать потоку умереть: иначе очередь растёт без границ.
				}
			}
		}

		private bool DrainQueueLocked()
		{
			var hasWritten = false;

			while(_queue.TryDequeue(out var line))
			{
				if(_writer == null)
					continue;

				try
				{
					_writer.WriteLine(line);
					_writtenBytes += Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;
					hasWritten = true;
				}
				catch(ObjectDisposedException)
				{
					return hasWritten;
				}
				catch(IOException)
				{
					return hasWritten;
				}

				if(_writtenBytes >= MaxFileBytes)
					RotateLocked();
			}

			return hasWritten;
		}

		private void RotateLocked()
		{
			CloseWriterLocked();
			OpenNextFile();
		}

		private void CloseWriterLocked()
		{
			if(_writer == null)
				return;

			try
			{
				_writer.Flush();
				_writer.Dispose();
			}
			catch(ObjectDisposedException)
			{
			}
			catch(IOException)
			{
			}

			_writer = null;
		}

		private void OpenNextFile()
		{
			_partIndex++;
			_writtenBytes = 0;

			var fileName = LogFileLocation.BuildFileName(_filePrefix, _sessionTime, _partIndex);
			var path = Path.Combine(_folderPath, fileName);

			try
			{
				// FileShare.ReadWrite — иначе Log Viewer и сборщик баг-репортов получат IOException
				var stream = new FileStream(
						path,
						FileMode.Create,
						FileAccess.Write,
						FileShare.ReadWrite,
						WriterBufferBytes
				);

				_writer = new StreamWriter(stream, new UTF8Encoding(false), WriterBufferBytes)
				{
						AutoFlush = false
				};

				CurrentLogFilePath = path;
				_sessionFilePaths.Add(path);
			}
			catch(Exception)
			{
				// Логировать здесь нечем — логгер и есть падающий компонент
				_writer = null;
			}
		}

		private void CleanupOldFiles()
		{
			try
			{
				var files = new DirectoryInfo(_folderPath).GetFiles("*.log");
				if(files.Length == 0)
					return;

				Array.Sort(files, (left, right) => right.LastWriteTimeUtc.CompareTo(left.LastWriteTimeUtc));

				long totalBytes = 0;
				for(var i = 0; i < files.Length; i++)
				{
					totalBytes += files[i].Length;

					var isOverCount = i >= MaxSessionFiles - 1;
					var isOverSize = totalBytes >= MaxTotalBytes;

					if(!isOverCount && !isOverSize)
						continue;

					try
					{
						files[i].Delete();
					}
					catch(IOException)
					{
					}
					catch(UnauthorizedAccessException)
					{
					}
				}
			}
			catch(Exception)
			{
			}
		}

		private static string FormatLine(in LogEntry entry)
		{
			var time = entry.UtcTime.ToLocalTime().ToString("HH:mm:ss.fff");
			var level = ToToken(entry.Level);
			var tag = string.IsNullOrEmpty(entry.Tag) ? "-" : entry.Tag;

			return $"[{time}] [{level}] [T{entry.ThreadId}] [{tag}] {entry.Message}";
		}

		private static string FormatStackTrace(string stackTrace)
			=> stackTrace.TrimEnd();

		private static string ToToken(LogLevel level)
			=> level switch
			{
					LogLevel.Verbose => "VERB",
					LogLevel.Info => "INFO",
					LogLevel.Warning => "WARN",
					LogLevel.Error => "ERROR",
					_ => "INFO"
			};
	}
}
