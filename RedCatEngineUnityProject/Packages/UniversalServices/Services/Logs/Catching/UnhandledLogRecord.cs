using System;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs.Catching
{
	public readonly struct UnhandledLogRecord
	{
		public readonly DateTime UtcTime;
		public readonly LogType Type;
		public readonly string Condition;
		public readonly string StackTrace;
		public readonly int RepeatCount;

		public UnhandledLogRecord(DateTime utcTime, LogType type, string condition, string stackTrace, int repeatCount)
		{
			UtcTime = utcTime;
			Type = type;
			Condition = condition;
			StackTrace = stackTrace;
			RepeatCount = repeatCount;
		}
	}
}
