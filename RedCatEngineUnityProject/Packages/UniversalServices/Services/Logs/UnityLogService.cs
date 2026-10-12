using RedCatEngine.CommonServices.Services.Logs.Filtering;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs
{
	/// <summary>
	/// Пишет напрямую в Unity-консоль, минуя синки. Нужен там, где нет полной инфраструктуры:
	/// тесты, editor-утилиты, ранняя инициализация.
	/// </summary>
	public class UnityLogService : BaseLogService
	{
		public UnityLogService(ILogLevelFilter filter = null)
				: base(null, filter)
		{
		}

		protected UnityLogService(string tag, ILogLevelFilter filter)
				: base(tag, filter)
		{
		}

		protected override ILogService DoCreateInstance(string tag)
			=> new UnityLogService(tag, Filter);

		protected override void DoWrite(LogLevel level, string message)
		{
			var text = string.IsNullOrEmpty(Tag)
					? message
					: $"[{Tag}] {message}";

			switch(level)
			{
				case LogLevel.Warning:
					Debug.LogWarning(text);
					break;
				case LogLevel.Error:
					Debug.LogError(text);
					break;
				case LogLevel.Verbose:
				case LogLevel.Info:
				case LogLevel.Off:
				default:
					Debug.Log(text);
					break;
			}
		}
	}
}
