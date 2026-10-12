using System;

namespace RedCatEngine.CommonServices.Services.Save.Data
{
	[Serializable]
	public class SaveData
	{
		public string SaveName;
		public ServiceSaveData[] ServicesSaveDatas;

		public SaveData(string saveName, ServiceSaveData[] servicesSaveDatas)
		{
			SaveName = saveName;
			ServicesSaveDatas = servicesSaveDatas;
		}
	}
}