using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using RedCatEngine.CommonServices.Services.Logs.Filtering;
using RedCatEngine.CommonServices.Services.Logs.Sinks;

namespace RedCatEngine.CommonServices.Services.Logs
{
	/// <summary>
	/// Единая точка, через которую сообщения уходят в синки. Здесь же счётчики по подсистемам —
	/// именно они дают объективный ответ «кто сейчас главный источник объёма».
	/// </summary>
	public sealed class LogRouter
	{
		private readonly ILogSink _sink;
		private readonly object _statsLock = new();
		private readonly Dictionary<string, long> _writtenByRootTag = new();

		public LogRouter(ILogLevelFilter filter, ILogSink sink)
		{
			Filter = filter ?? AlwaysAllowFilter.Instance;
			_sink = sink;
		}

		public ILogLevelFilter Filter { get; }

		public void Write(LogLevel level, string tag, string message)
		{
			if(_sink == null)
				return;

			CountWritten(tag);

			_sink.Write(
					new LogEntry(
							level,
							tag,
							message,
							level >= LogLevel.Error ? CaptureStackTrace() : null,
							DateTime.UtcNow,
							Thread.CurrentThread.ManagedThreadId,
							false
					)
			);
		}

		public void Flush()
			=> _sink?.Flush();

		/// <summary>
		/// Стек снимаем сами, а не ждём его от logMessageReceived: иначе при выключенном
		/// дублировании в Unity-консоль (log_unity false) свои ошибки уходили бы в файл без стека.
		/// Пропускаем кадры самого логгера.
		/// </summary>
		private static string CaptureStackTrace()
		{
			try
			{
				return new StackTrace(3, true).ToString();
			}
			catch(Exception)
			{
				return null;
			}
		}

		public string DumpStats()
		{
			lock(_statsLock)
			{
				if(_writtenByRootTag.Count == 0)
					return "Нет записанных сообщений.";

				var pairs = new List<KeyValuePair<string, long>>(_writtenByRootTag);
				pairs.Sort((left, right) => right.Value.CompareTo(left.Value));

				var builder = new StringBuilder("Записано сообщений по подсистемам:");
				foreach(var pair in pairs)
					builder.Append('\n').Append(pair.Key).Append(": ").Append(pair.Value);

				return builder.ToString();
			}
		}

		public void ResetStats()
		{
			lock(_statsLock)
				_writtenByRootTag.Clear();
		}

		private void CountWritten(string tag)
		{
			var rootTag = GetRootSegment(tag);

			lock(_statsLock)
			{
				_writtenByRootTag.TryGetValue(rootTag, out var current);
				_writtenByRootTag[rootTag] = current + 1;
			}
		}

		private static string GetRootSegment(string tag)
		{
			if(string.IsNullOrEmpty(tag))
				return "-";

			var separator = tag.IndexOf(':');
			return separator < 0
					? tag
					: tag.Substring(0, separator);
		}
	}
}
