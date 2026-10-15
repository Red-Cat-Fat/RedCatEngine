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
		// BaseGameSetting is abstract: only [SerializeReference] keeps the concrete setting types
		[SerializeReference]
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
			if (data._overrides == null)
				return;

			foreach (var setting in data._overrides)
			{
				if (setting != null)
					_overrides.Add(setting);
			}
		}

		public void AddOverride(BaseGameSetting gameSetting)
		{
			// one override per setting: a new value replaces the previous one
			_overrides.RemoveAll(setting => setting == null || setting.SaveKey == gameSetting.SaveKey);
			_overrides.Add(gameSetting);
		}

		public void Reset()
		{
			_overrides.Clear();
			Save();
		}
	}
}