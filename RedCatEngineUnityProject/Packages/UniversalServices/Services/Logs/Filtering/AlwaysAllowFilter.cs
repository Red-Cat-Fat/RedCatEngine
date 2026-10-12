using System.Collections.Generic;

namespace RedCatEngine.CommonServices.Services.Logs.Filtering
{
	/// <summary>
	/// Пропускает всё. Дефолт для логгеров, созданных вне DI (тесты, editor-утилиты).
	/// </summary>
	public sealed class AlwaysAllowFilter : ILogLevelFilter
	{
		public static readonly AlwaysAllowFilter Instance = new();

		private static readonly Dictionary<string, LogLevel> EmptyOverrides = new();
		private static readonly string[] EmptyTags = new string[0];

		private AlwaysAllowFilter()
		{
		}

		public int Version => 0;
		public LogLevel GlobalLevel => LogLevel.Verbose;

		public IReadOnlyCollection<string> KnownTags => EmptyTags;
		public IReadOnlyDictionary<string, LogLevel> Overrides => EmptyOverrides;

		public LogLevel Resolve(string tag) => LogLevel.Verbose;

		public void SetGlobalLevel(LogLevel level)
		{
		}

		public void SetLevel(string tagPrefix, LogLevel level)
		{
		}

		public void ClearLevel(string tagPrefix)
		{
		}

		public void Reset()
		{
		}

		public void Save()
		{
		}

		public void RegisterTag(string tag)
		{
		}
	}
}
