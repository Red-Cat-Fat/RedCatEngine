using RedCatEngine.CommonServices.Services.Logs;
using RedCatEngine.CommonServices.Services.Save.Applier;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Save.SaveReader
{
	public class PlayerPrefSaveLoadReaderService : BaseSaveLoadReaderService
	{
		[Inject]
		public PlayerPrefSaveLoadReaderService(
			ISaveApplierContainer saveApplierContainer,
			ILogService logService
		)
			: base(
				saveApplierContainer,
				logService
			)
		{
		}

		protected override string GetJsonForLoad(string key)
			=> PlayerPrefs.GetString(key);

		protected override bool SetJsonForSave(string key, string loadJson)
		{
			PlayerPrefs.SetString(key, loadJson);
			return true;
		}
	}
}