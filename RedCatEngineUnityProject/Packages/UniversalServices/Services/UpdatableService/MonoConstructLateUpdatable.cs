using RedCatEngine.CommonServices.Services.Times.TimeServices;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Specials.Components;

namespace RedCatEngine.CommonServices.Services.UpdatableService
{
	public abstract class MonoConstructLateUpdatable : MonoConstruct, ILateUpdatable
	{
		private IUpdatableService _updatableService;
		protected ITimeService TimeService;

		[MonoInject]
		public void Construct(IUpdatableService updatableService, ITimeService timeService)
		{
			TimeService = timeService;
			_updatableService = updatableService;
			_updatableService?.AddLateUpdatable(this);
		}

		protected sealed override void DoInitialize()
		{
			_updatableService?.AddLateUpdatable(this);
			DoSubscribeAfterInitialize();
		}

		/// <summary>
		/// Вызывается после инициализации всех компонентов через DI
		/// </summary>
		protected virtual void DoSubscribeAfterInitialize()
		{
		}

		/// <summary>
		/// Вызывается после вызова Dispose из DI
		/// </summary>
		protected virtual void DoUnsubscribeAfterDispose()
		{
		}

		protected sealed override void DoDispose()
		{
			_updatableService?.RemoveLateUpdated(this);
			DoUnsubscribeAfterDispose();
		}

		public virtual void OnLateUpdate()
		{
			if(TimeService == null)
				return;

			var deltaTime = TimeService.DeltaTime;
			DoLateUpdate(deltaTime);
		}

		protected abstract void DoLateUpdate(float deltaTime);
	}
}
