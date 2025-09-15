using System;
using RedCatEngine.GameSettings.TypeSettings;

namespace RedCatEngine.GameSettings
{
	public class GameSettingService
	{
		public event Action<BaseGameSetting> GameSettingChangeEvent;

		private readonly OverrideSettingsData _overrideSettingsData;
		
		public void OverrideGameSetting(BaseGameSetting gameSetting)
		{
			_overrideSettingsData.AddOverride(gameSetting);
			ApplySettingChange(gameSetting);
			GameSettingChangeEvent?.Invoke(gameSetting);
		}

		private void ApplySettingChange(BaseGameSetting gameSetting)
		{
			gameSetting.Apply();
		}

		public void LoadGameSetting()
		{
			var settingsConfig = GameSettingsConfig.Instance;
			foreach (var baseSetting in settingsConfig.BaseSettings)
				ApplySettingChange(baseSetting);

			_overrideSettingsData.Load();
			foreach (var baseSetting in _overrideSettingsData.Overrides)
				ApplySettingChange(baseSetting);
		}

		public void SaveSettings()
		{
			_overrideSettingsData.Save();
		}

		public void ResetSettings()
		{
			_overrideSettingsData.Reset();
			LoadGameSetting();
		}
	}
}