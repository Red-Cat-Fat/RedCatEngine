using RedCatEngine.CommonServices.Services.Logs;

namespace RedCatEngine.CommonServices.Services.UpdatableService
{
	public abstract class TestedMonoConstructUpdatable : MonoConstructUpdatable
	{
		private bool _sendLog;

		public sealed override void OnUpdate()
		{
#if UNITY_EDITOR
			base.OnUpdate();
#else
			SendLog();
#endif
		}

		private void SendLog()
		{
			if(_sendLog)
				return;

			LogHub.Root.LogErrorFormat("Tested element in build: {0}", gameObject.name);
			_sendLog = true;
		}
	}
}
