using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs
{
	public class UnityLogService : BaseLogService
	{
		public UnityLogService() : base(null)
		{
		}

		protected UnityLogService(string tag) : base(tag)
		{
		}

		protected override ILogService DoCreateInstance(string tag)
		{
			return new UnityLogService(tag);
		}

		protected override void DoLog(string log)
			=> Debug.Log(log);

		protected override void DoLogWarning(string log) 
			=> Debug.LogWarning(log);

		protected override void DoLogError(string log)
			=> Debug.LogError(log);
	}
}