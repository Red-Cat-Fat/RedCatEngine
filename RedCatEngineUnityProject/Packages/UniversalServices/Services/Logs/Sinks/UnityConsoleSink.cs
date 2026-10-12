using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs.Sinks
{
	/// <summary>
	/// Пишет в Unity-консоль. На время вызова взводит [ThreadStatic]-флаг, по которому
	/// UnityLogCatcher отличает собственные сообщения от чужих и не пишет их в файл дважды.
	/// Работает потому, что Application.logMessageReceivedThreaded вызывается синхронно
	/// на том же потоке, что и Debug.Log.
	/// </summary>
	public sealed class UnityConsoleSink : ILogSink
	{
		[System.ThreadStatic] private static bool _isEmitting;

		public static bool IsEmittingOwnLog => _isEmitting;

		public bool IsEnabled { get; set; } = true;

		public void Write(in LogEntry entry)
		{
			if(!IsEnabled)
				return;

			var message = Format(in entry);

			_isEmitting = true;
			try
			{
				switch(entry.Level)
				{
					case LogLevel.Warning:
						Debug.LogWarning(message);
						break;
					case LogLevel.Error:
						Debug.LogError(message);
						break;
					default:
						Debug.Log(message);
						break;
				}
			}
			finally
			{
				_isEmitting = false;
			}
		}

		public void Flush()
		{
		}

		private static string Format(in LogEntry entry)
			=> string.IsNullOrEmpty(entry.Tag)
					? entry.Message
					: $"[{entry.Tag}] {entry.Message}";
	}
}
