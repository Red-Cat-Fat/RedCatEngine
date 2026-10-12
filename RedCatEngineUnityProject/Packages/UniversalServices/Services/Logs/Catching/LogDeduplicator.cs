using System;
using System.Collections.Generic;

namespace RedCatEngine.CommonServices.Services.Logs.Catching
{
	/// <summary>
	/// Схлопывает повторяющиеся сообщения. Без этого один NRE в Update() даёт гигабайт
	/// в новом файле ровно так же, как давал в старом.
	/// Первые N вхождений пишем полностью, дальше — одна строка раз в интервал.
	/// </summary>
	public sealed class LogDeduplicator
	{
		private const int MaxTrackedKeys = 512;
		private const int FullWriteCount = 3;
		private readonly Dictionary<int, Entry> _entries = new();

		private readonly object _lock = new();
		private readonly long _reportIntervalTicks;

		public LogDeduplicator(TimeSpan? reportInterval = null)
			=> _reportIntervalTicks = (reportInterval ?? TimeSpan.FromSeconds(5)).Ticks;

		/// <summary>
		/// true — сообщение надо записать. <paramref name="suppressedCount"/> > 0 означает,
		/// что это периодический отчёт о подавленных повторах.
		/// </summary>
		public bool ShouldWrite(string condition, string stackTrace, out int suppressedCount)
		{
			suppressedCount = 0;

			var key = BuildKey(condition, stackTrace);
			var nowTicks = DateTime.UtcNow.Ticks;

			lock(_lock)
			{
				if(_entries.Count >= MaxTrackedKeys)
					_entries.Clear();

				if(!_entries.TryGetValue(key, out var entry))
				{
					_entries[key] = new Entry
					{
							Count = 1,
							ReportedCount = 1,
							LastReportTicks = nowTicks
					};
					return true;
				}

				entry.Count++;

				if(entry.Count <= FullWriteCount)
				{
					entry.ReportedCount = entry.Count;
					entry.LastReportTicks = nowTicks;
					_entries[key] = entry;
					return true;
				}

				if(nowTicks - entry.LastReportTicks < _reportIntervalTicks)
				{
					_entries[key] = entry;
					return false;
				}

				suppressedCount = entry.Count - entry.ReportedCount;
				entry.ReportedCount = entry.Count;
				entry.LastReportTicks = nowTicks;
				_entries[key] = entry;
				return true;
			}
		}

		public void Clear()
		{
			lock(_lock)
				_entries.Clear();
		}

		private static int BuildKey(string condition, string stackTrace)
		{
			var conditionHash = condition?.GetHashCode() ?? 0;
			var stackHash = FirstLinesHash(stackTrace, 2);

			unchecked
			{
				return conditionHash ^ (stackHash * 397);
			}
		}

		private static int FirstLinesHash(string stackTrace, int lineCount)
		{
			if(string.IsNullOrEmpty(stackTrace))
				return 0;

			var index = 0;
			for(var line = 0; line < lineCount; line++)
			{
				var next = stackTrace.IndexOf('\n', index);
				if(next < 0)
				{
					index = stackTrace.Length;
					break;
				}

				index = next + 1;
			}

			return stackTrace.Substring(0, index).GetHashCode();
		}

		private struct Entry
		{
			public int Count;
			public int ReportedCount;
			public long LastReportTicks;
		}
	}
}
