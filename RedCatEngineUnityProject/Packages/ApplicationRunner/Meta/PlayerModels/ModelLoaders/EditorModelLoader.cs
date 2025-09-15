using System;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using UnityEngine;

namespace RedCatEngine.ApplicationRunner.Meta.PlayerModels.ModelLoaders
{
	public class EditorModelLoader<TModelData> : IModelLoader<TModelData>
		where TModelData : class, new()
	{
		private const string SaveKey = "GameSave";

		[Inject]
		public EditorModelLoader()
		{
			
		}
		
		public void Save(TModelData modelForSave)
		{
			var save = JsonUtility.ToJson(modelForSave);
			Debug.Log($"Save model: {save}");
			PlayerPrefs.SetString(SaveKey, save);
		}

		public void Load(Action<TModelData> callbackLoad)
		{
			var save = PlayerPrefs.GetString(SaveKey);
			if (!string.IsNullOrEmpty(save))
				callbackLoad?.Invoke(JsonUtility.FromJson<TModelData>(save));
			callbackLoad?.Invoke(new TModelData());
		}
	}
}