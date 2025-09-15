using System.IO;
using UnityEngine;

namespace RedCatEngine.GameSettings.Help
{
	public class SettingsSerializer<TSerializedData> where TSerializedData : new()
	{
		private readonly string _projectName = Application.productName;
		private readonly string _settingsFilePath;

		public SettingsSerializer(string settingsFilePath, string fileName)
		{
			_settingsFilePath = Path.Combine(settingsFilePath, $"{_projectName}_{fileName}.json");
		}

		public bool TryLoadDataFrom(out TSerializedData data)
		{
			if (!File.Exists(_settingsFilePath))
			{
				data = new TSerializedData();
				Save(data);
				return true;
			}

			var json = File.ReadAllText(_settingsFilePath);
			data = LoadDataFrom(json);
			return data != null;
		}

		public void Save(TSerializedData data)
		{
			File.WriteAllText(_settingsFilePath, GetDataFrom(data));
		}

		private string GetDataFrom(TSerializedData data)
			=> JsonUtility.ToJson(data);

		private TSerializedData LoadDataFrom(string json)
			=> string.IsNullOrEmpty(json)
				? new TSerializedData()
				: JsonUtility.FromJson<TSerializedData>(json);
	}
}