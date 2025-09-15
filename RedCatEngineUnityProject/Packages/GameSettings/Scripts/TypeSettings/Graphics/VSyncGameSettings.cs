using System;
using RedCatEngine.GameSettings.Help;
using SerializeReferenceEditor;

namespace RedCatEngine.GameSettings.TypeSettings.Graphics
{
	[Serializable]
	[SRName("Graphics/VSync")]
	public class VSyncGameSettings : BoolGameSettings
	{
		public override string SaveKey
			=> nameof(VSyncGameSettings);


		public VSyncGameSettings() : base(false)
		{
		}

		public VSyncGameSettings(bool vSyncValue) : base(vSyncValue)
		{
		}

		protected override void DoApply()
		{
			GraphicsSettingsApplier.SetVSync(Value);
		}
	}
}