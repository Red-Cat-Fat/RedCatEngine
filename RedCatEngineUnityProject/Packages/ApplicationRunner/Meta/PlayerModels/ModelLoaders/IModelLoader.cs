using System;

namespace RedCatEngine.ApplicationRunner.Meta.PlayerModels.ModelLoaders
{
	public interface IModelLoader<TModelData>
		where TModelData : class, new()
	{
		void Save(TModelData modelForSave);
		void Load(Action<TModelData> callbackLoad);
	}
}