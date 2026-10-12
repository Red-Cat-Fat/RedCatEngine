using System;
using RedCatEngine.CommonServices.Services.Save.Data;
using RedCatEngine.CommonServices.Services.Save.SaveReader;
using RedCatEngine.DependencyInjection.Containers.Attributes;

namespace RedCatEngine.CommonServices.Services.Save
{
	public class PlayerSaveService
	{
		private const string SavePath = "Saves";
		private readonly ISaveLoadReaderService _saveLoadService;

		[Inject]
		public PlayerSaveService(ISaveLoadReaderService saveLoadService)
		{
			_saveLoadService = saveLoadService;
		}

		public void Save()
		{
			_saveLoadService.SaveCurrentState(SavePath);
		}

		public void Load()
		{
			_saveLoadService.Load(SavePath);
		}

		public void RemoveSave()
		{
			_saveLoadService.Save(SavePath, new SaveData(SavePath, Array.Empty<ServiceSaveData>()));
		}
	}
}