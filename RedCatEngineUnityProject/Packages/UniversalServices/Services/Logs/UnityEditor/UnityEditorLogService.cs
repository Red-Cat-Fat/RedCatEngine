namespace RedCatEngine.CommonServices.Services.Logs.UnityEditor
{
	public class UnityEditorLogService : UnityLogService
	{
		public UnityEditorLogService()
		{
		}

		protected UnityEditorLogService(string tag)
			: base(tag)
		{
		}

#if UNITY_EDITOR
		private bool IsCanShow()
			=> UnityEditorLogServiceStaticBridge.IsLogTypeEnabled(Tag);

		protected override void DoLog(string log)
		{
			if (IsCanShow())
				base.DoLog(log);
		}

		protected override void DoLogWarning(string log)
		{
			if (IsCanShow())
				base.DoLogWarning(log);
		}

		protected override void DoLogError(string log)
		{
			if (IsCanShow())
				base.DoLogError(log);
		}

		protected override ILogService DoCreateInstance(string tag)
		{
			UnityEditorLogServiceStaticBridge.AddLog(tag);
			return new UnityEditorLogService(tag);
		}
#endif
	}
}