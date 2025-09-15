#if YANDEX_GAME
using System;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using UnityEngine;
using YG;

namespace RedCatEngine.ApplicationRunner.Meta.PlayerModels.ModelLoaders
{
	public class YandexModelLoader<TModelData> : IModelLoader<TModelData>, IDisposable
		where TModelData : class, new()
	{
		private Action<TModelData> _onLoadCallBack;

		[Inject]
		public YandexModelLoader()
		{
			
		}
		
		public void Save(TModelData modelForSave)
		{
			YandexGame.savesData.SavedModel = JsonUtility.ToJson(modelForSave);
			YandexGame.SaveProgress();
		}

		public void Load(Action<TModelData> callbackLoad)
		{
			_onLoadCallBack = callbackLoad;
			if (!YandexGame.SDKEnabled)
			{
				YandexGame.GetDataEvent += OnGetData;
				return;
			}

			OnGetData();
		}

		private void OnGetData()
		{
			var save = YandexGame.savesData.SavedModel;
			if (!string.IsNullOrEmpty(save))
				_onLoadCallBack?.Invoke(JsonUtility.FromJson<TModelData>(save));
			_onLoadCallBack?.Invoke(new TModelData());
		}

		public void Dispose()
		{
			YandexGame.GetDataEvent -= OnGetData;
		}
	}
}
#endif