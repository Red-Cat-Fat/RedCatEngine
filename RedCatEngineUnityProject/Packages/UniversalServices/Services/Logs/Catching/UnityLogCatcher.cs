using System;
using System.Threading;
using RedCatEngine.CommonServices.Services.Logs.Sinks;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs.Catching
{
	/// <summary>
	/// Ловит всё, что проходит через Unity: NRE и прочие исключения, логи Mirror/Wwise,
	/// голые Debug.Log в чужом коде — и кладёт в собственный файл.
	///
	/// Подписка на logMessageReceivedThreaded (не logMessageReceived): обработчик вызывается
	/// синхронно на потоке-источнике, поэтому [ThreadStatic]-флаг UnityConsoleSink надёжно
	/// отличает наши собственные сообщения от чужих.
	/// </summary>
	public sealed class UnityLogCatcher : IDisposable
	{
		[ThreadStatic] private static bool _inHandler;
		private readonly UnhandledLogBuffer _buffer;
		private readonly LogDeduplicator _deduplicator;

		private readonly FileLogSink _fileSink;

		private bool _isSubscribed;

		public UnityLogCatcher(FileLogSink fileSink, UnhandledLogBuffer buffer, LogDeduplicator deduplicator)
		{
			_fileSink = fileSink;
			_buffer = buffer;
			_deduplicator = deduplicator;
		}

		public void Dispose()
			=> Unsubscribe();

		public void Subscribe()
		{
			if(_isSubscribed)
				return;

			Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
			_isSubscribed = true;
		}

		public void Unsubscribe()
		{
			if(!_isSubscribed)
				return;

			Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
			_isSubscribed = false;
		}

		private void OnLogMessageReceivedThreaded(string condition, string stackTrace, LogType type)
		{
			// Исключение внутри обработчика логов = бесконечная рекурсия
			if(_inHandler)
				return;

			_inHandler = true;
			try
			{
				var level = MapLevel(type);
				var isOwn = UnityConsoleSink.IsEmittingOwnLog;

				if(level >= LogLevel.Error)
					_buffer.Push(new UnhandledLogRecord(DateTime.UtcNow, type, condition, stackTrace, 1));

				// Своё сообщение уже записано роутером — вместе со стеком
				if(isOwn)
					return;

				if(!_deduplicator.ShouldWrite(condition, stackTrace, out var suppressedCount))
					return;

				var message = suppressedCount > 0
						? $"(повторилось {suppressedCount} раз) {condition}"
						: condition;

				_fileSink.Write(
						new LogEntry(
								level,
								"UNITY",
								message,
								level >= LogLevel.Error ? stackTrace : null,
								DateTime.UtcNow,
								Thread.CurrentThread.ManagedThreadId,
								true
						)
				);
			}
			catch(Exception)
			{
				// Молча: любое исключение отсюда уйдёт обратно в этот же обработчик
			}
			finally
			{
				_inHandler = false;
			}
		}

		private static LogLevel MapLevel(LogType type)
			=> type switch
			{
					LogType.Log => LogLevel.Info,
					LogType.Warning => LogLevel.Warning,
					LogType.Assert => LogLevel.Error,
					LogType.Error => LogLevel.Error,
					LogType.Exception => LogLevel.Error,
					_ => LogLevel.Info
			};
	}
}
