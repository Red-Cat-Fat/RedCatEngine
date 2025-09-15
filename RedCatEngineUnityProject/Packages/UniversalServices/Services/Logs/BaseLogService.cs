namespace RedCatEngine.CommonServices.Services.Logs
{
	public abstract class BaseLogService : ILogService
	{
		protected readonly string Tag;

		protected BaseLogService(string tag)
		{
			Tag = tag;
		}

		public void Log(string log)
			=> DoLog(
				string.IsNullOrEmpty(Tag)
					? log
					: $"[{Tag}] {log}"
			);

		public void LogWarning(string log)
			=> DoLogWarning(
				string.IsNullOrEmpty(Tag)
					? log
					: $"[{Tag}] {log}"
			);

		public void LogError(string log)
			=> DoLogError(
				string.IsNullOrEmpty(Tag)
					? log
					: $"[{Tag}] {log}"
			);

		public void LogFormat(string log, params object[] parameters)
		{
			var logFormat = string.Format(log, parameters);
			Log(logFormat);
		}

		public void LogWarningFormat(string log, params object[] parameters)
		{
			var logFormat = string.Format(log, parameters);
			LogWarning(logFormat);
		}

		public void LogErrorFormat(string log, params object[] parameters)
		{
			var logFormat = string.Format(log, parameters);
			LogError(logFormat);
		}

		public ILogService CreateTag<TType>()
			=> CreateTag(typeof(TType).Name);

		public ILogService CreateTag(string tag)
		{
			var tagForInstance = string.IsNullOrEmpty(Tag)
				? tag
				: $"{Tag}:{tag}";
			return DoCreateInstance(tagForInstance);
		}

		protected abstract ILogService DoCreateInstance(string tag);

		protected abstract void DoLog(string log);
		protected abstract void DoLogWarning(string log);
		protected abstract void DoLogError(string log);
	}
}