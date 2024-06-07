namespace RedCatEngine.StateMachine.StateMachines
{
	public interface IPayloadedState : IExitableState
	{
		void Enter(object payload);
	}

	public interface IPayloadedState<in TPayload> : IPayloadedState
	{
		void Enter(TPayload payload);
	}
}