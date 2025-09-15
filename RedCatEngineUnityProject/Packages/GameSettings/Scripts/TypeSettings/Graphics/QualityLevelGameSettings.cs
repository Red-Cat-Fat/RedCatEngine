using System;
using RedCatEngine.GameSettings.Help;
using SerializeReferenceEditor;

namespace RedCatEngine.GameSettings.TypeSettings.Graphics
{
	[Serializable]
	[SRName("Graphics/Quality Level")]
	public class QualityLevelGameSettings : BaseGameSetting
	{
		public override string SaveKey
			=> nameof(QualityLevelGameSettings);

		public int Level;

		public QualityLevelGameSettings()
		{
		}

		public QualityLevelGameSettings(
			int level
		)
		{
			Level = level;
		}

		protected override void DoApply()
		{
			GraphicsSettingsApplier.SetQualityLevel(Level, true);
		}
	}
}