using System;

namespace RedCatEngine.CommonServices.Services.UpdatableService
{
	public interface IUpdateTrigger
	{
		event Action UpdateEvent;
		event Action FixedUpdateEvent;
		event Action LateUpdateEvent;
	}
}