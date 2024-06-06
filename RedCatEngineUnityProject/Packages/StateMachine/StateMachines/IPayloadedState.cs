namespace RedCatEngine.StateMachine.StateMachines
{
	public interface IPayloadedState : IExitableState
	{
		void Enter(object payload);
	}

	public interface IPayloadedState<in TPayload> : IPayloadedState
	{
		new void Enter(object payload)
			=> Enter((TPayload)payload);

		void Enter(TPayload payload);
	}
}