using System;

namespace RedCatEngine.CommonServices.Services.Logs.Sinks
{
	/// <summary>
	/// Одно сообщение на пути к синкам. readonly struct + передача по in — чтобы не аллоцировать.
	/// </summary>
	public readonly struct LogEntry
	{
		public readonly LogLevel Level;
		public readonly string Tag;
		public readonly string Message;

		/// <summary>null для обычных сообщений; заполнен для перехваченных из Unity.</summary>
		public readonly string StackTrace;

		public readonly DateTime UtcTime;
		public readonly int ThreadId;

		/// <summary>true — пришло из UnityLogCatcher, а не через ILogService.</summary>
		public readonly bool IsExternal;

		public LogEntry(
				LogLevel level,
				string tag,
				string message,
				string stackTrace,
				DateTime utcTime,
				int threadId,
				bool isExternal
		)
		{
			Level = level;
			Tag = tag;
			Message = message;
			StackTrace = stackTrace;
			UtcTime = utcTime;
			ThreadId = threadId;
			IsExternal = isExternal;
		}
	}
}
