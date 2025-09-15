using RedCatEngine.ApplicationRunner.Meta.PlayerModels;
using RedCatEngine.ApplicationRunner.Meta.PlayerModels.ModelLoaders;
using RedCatEngine.ApplicationRunner.Meta.PlayerModels.SaveTriggers;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;
using RedCatEngine.StateMachine.StateMachines;

namespace RedCatEngine.ApplicationRunner.Infrastructure.States.LoadModelStates
{
	public class LoadPlayerModelState<TModelData, TModelLoader> : ILoadPlayerModelState
		where TModelData : class, new()
		where TModelLoader : IModelLoader<TModelData>
	{
		private readonly IUnityGameContainer _applicationContainer;
		private readonly ITypedQueueStateMachine _stateMachine;
		private IPlayerModelContainer<TModelData> _playerModelContainer;

		[Inject]
		public LoadPlayerModelState(
			IUnityGameContainer applicationContainer,
			ITypedQueueStateMachine stateMachine
		)
		{
			_applicationContainer = applicationContainer;
			_stateMachine = stateMachine;
		}

		public void Enter()
		{
			_applicationContainer.BindType<IModelLoader<TModelData>, TModelLoader>();
			_applicationContainer.BindType<IPlayerModelSaveTrigger, PlayerModelSaveTrigger>();
			_playerModelContainer = _applicationContainer
				.BindType<IPlayerModelContainer<TModelData>, PlayerModelContainer<TModelData>>();
			_playerModelContainer.Load();
			if (!_playerModelContainer.IsReady)
				_playerModelContainer.ReadyEvent += OnReady;
			else
				OnSaveLoad();
		}

		private void OnReady()
		{
			_playerModelContainer.ReadyEvent -= OnReady;
			OnSaveLoad();
		}

		private void OnSaveLoad()
		{
			_applicationContainer.BindAsSingle(_playerModelContainer.Model);
			_stateMachine.EnterNextFromQueue();
		}

		public void Exit() { }
	}
}