using System;
using System.Globalization;
using RedCatEngine.CommonServices.Services.Logs.Filtering;

namespace RedCatEngine.CommonServices.Services.Logs
{
	/// <summary>
	/// Общая часть логгеров: композиция тегов и кэш эффективного уровня.
	/// Кэш инвалидируется по <see cref="ILogLevelFilter.Version"/>, поэтому стоимость
	/// выключенного вызова — чтение int и два сравнения, без словарей и строк.
	/// </summary>
	public abstract class BaseLogService : ILogService
	{
		private LogLevel _cachedLevel;
		private int _cachedVersion = -1;

		protected BaseLogService(string tag, ILogLevelFilter filter)
		{
			Tag = tag;
			Filter = filter ?? AlwaysAllowFilter.Instance;
		}

		protected ILogLevelFilter Filter { get; }

		public string Tag { get; }

		public bool IsVerbose => IsEnabled(LogLevel.Verbose);

		public bool IsEnabled(LogLevel level)
		{
			var version = Filter.Version;
			if(_cachedVersion == version)
				return level >= _cachedLevel;
			_cachedLevel = Filter.Resolve(Tag);
			_cachedVersion = version;

			return level >= _cachedLevel;
		}

		public ILogService CreateTag(string tag)
		{
			var tagForInstance = BuildTag(tag);
			Filter.RegisterTag(tagForInstance);
			return DoCreateInstance(tagForInstance);
		}

		public ILogService CreateTag<TType>()
			=> CreateTag(typeof(TType).Name);

		public ILogService CreateInstanceTag(string instanceName)
			=> DoCreateInstance(BuildTag(instanceName));

		public void Write(LogLevel level, string message)
		{
			if(!IsEnabled(level))
				return;

			DoWrite(level, message);
		}

		public void WriteException(Exception exception, string context)
		{
			var message = string.IsNullOrEmpty(context)
					? exception?.ToString() ?? "null exception"
					: $"{context}: {exception}";

			Write(LogLevel.Error, message);
		}

		public void LogWarning(string log)
			=> Write(LogLevel.Warning, log);

		public void LogError(string log)
			=> Write(LogLevel.Error, log);

		public void LogWarningFormat(string log, params object[] parameters)
		{
			if(!IsEnabled(LogLevel.Warning))
				return;

			DoWrite(LogLevel.Warning, string.Format(CultureInfo.CurrentCulture, log, parameters));
		}

		public void LogErrorFormat(string log, params object[] parameters)
		{
			if(!IsEnabled(LogLevel.Error))
				return;

			DoWrite(LogLevel.Error, string.Format(CultureInfo.CurrentCulture, log, parameters));
		}

		protected abstract ILogService DoCreateInstance(string tag);

		protected abstract void DoWrite(LogLevel level, string message);

		private string BuildTag(string tag)
			=> string.IsNullOrEmpty(Tag)
					? tag
					: $"{Tag}:{tag}";
	}
}
