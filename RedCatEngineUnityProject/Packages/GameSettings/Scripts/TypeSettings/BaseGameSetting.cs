using System;

namespace RedCatEngine.GameSettings.TypeSettings
{
	[Serializable]
	public abstract class BaseGameSetting
	{
		public abstract string SaveKey { get; }
		public int Id
			=> SaveKey.GetHashCode();
		
		protected virtual bool IsCanApply()
		{
			return true;
		}

		public void Apply()
		{
			if (IsCanApply())
				DoApply();
		}

		protected abstract void DoApply();
	}
}