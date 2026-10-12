using RedCatEngine.CommonServices.Extensions;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Save.Realisations
{
	public class SaveSerializer<TSaveDataType> where TSaveDataType : new()
	{
		public string SaveDataFrom(TSaveDataType data)
			=> JsonUtility.ToJson(data);

		public TSaveDataType LoadDataFrom(string json)
			=> json.IsNullOrEmpty() ? new TSaveDataType() : JsonUtility.FromJson<TSaveDataType>(json);
	}
}