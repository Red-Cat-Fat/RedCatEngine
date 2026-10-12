using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RedCatEngine.CommonServices.Services.Save.Data;
using RedCatEngine.CommonServices.Services.Save.Realisations;

namespace RedCatEngine.CommonServices.Services.Save.Applier
{
	[UsedImplicitly]
	public class SaveApplierContainer : ISaveApplierContainer
	{
		private readonly List<ISaved> _saveLoaders = new();

		public Type[] ExpectedTypes
			=> new[] { typeof(ISaved) };

		public void Attach(ISaved waitType)
			=> _saveLoaders.Add(waitType);

		public void Attach(IEnumerable<ISaved> waitTypes)
		{
			foreach (var saveLoader in waitTypes)
				Attach(saveLoader);
		}

		public void Load(SaveData save)
		{
			var data = save != null ? save.ServicesSaveDatas : Array.Empty<ServiceSaveData>();
			var loadedServices = new List<ISaved>();
			foreach (var saveData in data)
			{
				foreach (var saveLoader in _saveLoaders.Where(saveLoader => saveLoader.SaveKey == saveData.Key))
				{
					saveLoader.Load(saveData.Save);
					loadedServices.Add(saveLoader);
					break;
				}
			}

			foreach (var notLoadedService in _saveLoaders.Where(saveLoaders => !loadedServices.Contains(saveLoaders)))
			{
				notLoadedService.Load(null);
			}
		}

		public SaveData GenerateSaveWithName(string saveName)
		{
			return new SaveData(saveName, ServicesData());
		}

		private ServiceSaveData[] ServicesData()
		{
			var saveData = new List<ServiceSaveData>();

			foreach (var saveLoader in _saveLoaders)
				saveData.Add(new ServiceSaveData(saveLoader.SaveKey, saveLoader.GetSerializedSaveData()));
			return saveData.ToArray();
		}
	}
}