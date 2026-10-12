using System;

namespace RedCatEngine.CommonServices.Services.Save.Data
{
	[Serializable]
	public class ServiceSaveData
	{
		public string Key;
		public string Save;

		public ServiceSaveData(string key, string save)
		{
			Key = key;
			Save = save;
		}
	}
}