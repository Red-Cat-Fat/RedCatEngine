using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs
{
	public interface ILogService
	{
		ILogService CreateTag(string tag);
		ILogService CreateTag<TType>();
		void Log(string log);
		void LogWarning(string log);
		void LogError(string log);
		void LogFormat(string log, params object[] parameters);
		void LogWarningFormat(string log, params object[] parameters);
		void LogErrorFormat(string log, params object[] parameters);
		public ILogService CreateTag(GameObject gameObject)
			=> CreateTag(gameObject.name);
	}
}