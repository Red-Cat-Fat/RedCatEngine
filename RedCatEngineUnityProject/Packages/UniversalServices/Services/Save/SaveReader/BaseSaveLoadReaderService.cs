using RedCatEngine.CommonServices.Extensions;
using RedCatEngine.CommonServices.Services.Logs;
using RedCatEngine.CommonServices.Services.Save.Applier;
using RedCatEngine.CommonServices.Services.Save.Data;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Save.SaveReader
{
	public abstract class BaseSaveLoadReaderService : ISaveLoadReaderService
	{
		private readonly ILogService _logService;
		private readonly ISaveApplierContainer _saveApplierContainer;

		protected BaseSaveLoadReaderService(ISaveApplierContainer saveApplierContainer, ILogService logService)
		{
			_saveApplierContainer = saveApplierContainer;
			_logService = logService.CreateTag("SaveLoadService");
		}

		public void Load(string key)
		{
			var loadJson = GetJsonForLoad(key);
			if(string.IsNullOrEmpty(loadJson))
			{
				// first launch: every service starts from its defaults, then the initial state is written
				_logService.Info("{0} save is empty, so created save", key);
				_saveApplierContainer.Load(null);
				SaveCurrentState(key);
				return;
			}

			var save = JsonUtility.FromJson<SaveData>(loadJson);
			_saveApplierContainer.Load(save);
			_logService.Info(
					"{0} save is load ({2} kB): {1}",
					key,
					loadJson,
					loadJson.SizeOfMemoryKb()
			);
		}

		public SaveData LoadData(string key)
		{
			var loadJson = GetJsonForLoad(key);
			if(string.IsNullOrEmpty(loadJson))
			{
				_logService.Info("{0} save is empty", key);
				return null;
			}

			var save = JsonUtility.FromJson<SaveData>(loadJson);
			_logService.Info(
					"{0} save data read ({2} kB): {1}",
					key,
					loadJson,
					loadJson.SizeOfMemoryKb()
			);
			return save;
		}

		public bool Save(string key, SaveData save)
		{
			var loadJson = JsonUtility.ToJson(save);
			var ok = SetJsonForSave(key, loadJson);

			if(ok)
			{
				_logService.Info(
						"{0} saved ({2} kB): {1}",
						key,
						loadJson,
						loadJson.SizeOfMemoryKb()
				);
			}

			return ok;
		}

		public bool SaveCurrentState(string key)
			=> Save(key, _saveApplierContainer.GenerateSaveWithName(key));

		protected abstract string GetJsonForLoad(string key);
		protected abstract bool SetJsonForSave(string key, string loadJson);
	}
}
