using System;

namespace RedCatEngine.CommonServices.Services.Logs
{
	/// <summary>
	/// No-op логгер. Единственный экземпляр — чтобы не аллоцировать заглушку на каждый вызов,
	/// как это делал старый HideInBuild().
	/// </summary>
	public sealed class NullLogService : ILogService
	{
		public static readonly NullLogService Instance = new();

		private NullLogService()
		{
		}

		public string Tag => string.Empty;

		public ILogService CreateTag(string tag) => this;
		public ILogService CreateTag<TType>() => this;
		public ILogService CreateInstanceTag(string instanceName) => this;

		public bool IsEnabled(LogLevel level) => false;
		public bool IsVerbose => false;

		public void Write(LogLevel level, string message)
		{
		}

		public void WriteException(Exception exception, string context)
		{
		}

		public void LogWarning(string log)
		{
		}

		public void LogError(string log)
		{
		}

		public void LogWarningFormat(string log, params object[] parameters)
		{
		}

		public void LogErrorFormat(string log, params object[] parameters)
		{
		}
	}
}
