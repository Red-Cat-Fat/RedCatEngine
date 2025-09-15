using System;
using JetBrains.Annotations;
using RedCatEngine.ApplicationRunner.Meta.PlayerModels.ModelLoaders;
using RedCatEngine.ApplicationRunner.Meta.PlayerModels.SaveTriggers;
using RedCatEngine.DependencyInjection.Containers.Attributes;

namespace RedCatEngine.ApplicationRunner.Meta.PlayerModels
{
	public class PlayerModelContainer<TModelData> : IPlayerModelContainer<TModelData>, IDisposable
		where TModelData : class, new()
	{
		public event Action ReadyEvent;

		public bool IsReady
			=> Model != null;

		private readonly IModelLoader<TModelData> _loader;
		private readonly IPlayerModelSaveTrigger _playerModelSaveTrigger;
		public TModelData Model { get; private set; }

		[Inject]
		[UsedImplicitly]
		public PlayerModelContainer(IModelLoader<TModelData> loader, IPlayerModelSaveTrigger playerModelSaveTrigger)
		{
			_loader = loader;
			_playerModelSaveTrigger = playerModelSaveTrigger;
			_playerModelSaveTrigger.SaveEvent += Save;
		}

		public void Load()
		{
			_loader.Load(OnSaveLoaded);
		}

		public void Save()
		{
			_loader.Save(Model);
		}

		private void OnSaveLoaded(TModelData modelData)
		{
			Model = modelData;
			ReadyEvent?.Invoke();
		}

#if CHEAT_ENABLE
		public void ResetModel()
			=> _loader.Save(new TModelData());
#endif

		public void Dispose()
		{
			_playerModelSaveTrigger.SaveEvent -= Save;
		}
	}
}