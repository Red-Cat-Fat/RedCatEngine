using System;
using RedCatEngine.GameSettings.Help;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.GameSettings.TypeSettings.Graphics
{
	[Serializable]
	[SRName("Graphics/Resolution")]
	public class ResolutionGameSetting : BaseGameSetting
	{
		public override string SaveKey
			=> nameof(ResolutionGameSetting);

		public int Width = 1920;
		public int Height = 1080;
		public FullScreenMode Mode = FullScreenMode.ExclusiveFullScreen;
		public int RefreshRate = 60;

		public ResolutionGameSetting()
		{
		}

		public ResolutionGameSetting(
			int width,
			int height,
			FullScreenMode mode,
			int refreshRate
		)
		{
			Width = width;
			Height = height;
			Mode = mode;
			RefreshRate = refreshRate;
		}

		protected override void DoApply()
		{
			GraphicsSettingsApplier.SetResolution(
				Width,
				Height,
				Mode,
				RefreshRate);
		}
	}
}
