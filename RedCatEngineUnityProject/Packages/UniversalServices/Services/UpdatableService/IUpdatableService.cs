namespace RedCatEngine.CommonServices.Services.UpdatableService
{
	public interface IUpdatableService
	{
		void AddUpdatable(IUpdatable updatableElement);
		void AddUpdatable(IUpdatable[] updatedElements);
		void AddFixedUpdatable(IFixedUpdatable updatableElement);
		void AddFixedUpdatable(IFixedUpdatable[] updatedElements);
		void AddLateUpdatable(ILateUpdatable updatableElement);
		void AddLateUpdatable(ILateUpdatable[] updatedElements);
		void RemoveUpdated(IUpdatable updatableElement);
		void RemoveUpdated(IUpdatable[] updatedElements);
		void RemoveFixedUpdated(IFixedUpdatable updatableElement);
		void RemoveFixedUpdated(IFixedUpdatable[] updatedElements);
		void RemoveLateUpdated(ILateUpdatable updatableElement);
		void RemoveLateUpdated(ILateUpdatable[] updatedElements);
	}
}