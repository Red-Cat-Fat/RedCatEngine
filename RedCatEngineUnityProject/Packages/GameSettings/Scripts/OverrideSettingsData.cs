using System;
using System.Collections.Generic;
using RedCatEngine.GameSettings.Help;
using RedCatEngine.GameSettings.TypeSettings;
using UnityEngine;

namespace RedCatEngine.GameSettings
{
	[Serializable]
	public class OverrideSettingsData
	{
		[SerializeField]
		private List<BaseGameSetting> _overrides = new();

		private readonly SettingsSerializer<OverrideSettingsData> _overrideSettingsSaver
			= new(
				Application.persistentDataPath,
				"overrideSettingsData");

		public IReadOnlyList<BaseGameSetting> Overrides
			=> _overrides;

		public void Save()
		{
			_overrideSettingsSaver.Save(this);
		}

		public void Load()
		{
			if (!_overrideSettingsSaver.TryLoadDataFrom(out var data))
			{
				Debug.LogError("OverrideSettingsData could not be loaded.");
				return;
			}

			_overrides.Clear();
			_overrides.AddRange(data._overrides);
		}

		public void AddOverride(BaseGameSetting gameSetting)
		{
			_overrides.Add(gameSetting);
		}

		public void Reset()
		{
			_overrides.Clear();
			Save();
		}
	}
}