using System;
using RedCatEngine.GameSettings.Help;
using SerializeReferenceEditor;

namespace RedCatEngine.GameSettings.TypeSettings.Graphics
{
	[Serializable]
	[SRName("Graphics/Frame Rate Cap")]
	public class FrameRateCapGameSettings : BaseGameSetting
	{
		public override string SaveKey
			=> nameof(TargetFrameRate);

		public int TargetFrameRate = -1;

		public FrameRateCapGameSettings()
		{
		}

		public FrameRateCapGameSettings(int targetFrameRate)
		{
			TargetFrameRate = targetFrameRate;
		}

		protected override void DoApply()
		{
			GraphicsSettingsApplier.SetFrameRateCap(TargetFrameRate);
		}
	}
}