using System;
using RedCatEngine.GameSettings.Help;
using SerializeReferenceEditor;

namespace RedCatEngine.GameSettings.TypeSettings.Graphics
{
	[Serializable]
	[SRName("Graphics/Shadows Game Settings")]
	public class ShadowsGameSettings : BoolGameSettings
	{
		public override string SaveKey
			=> nameof(ShadowsGameSettings);


		public ShadowsGameSettings() : base(true)
		{
		}

		public ShadowsGameSettings(bool shadowsEnabled) : base(shadowsEnabled) 
		{
		}

		protected override void DoApply()
		{
			GraphicsSettingsApplier.SetShadows(Value);
		}
	}
}