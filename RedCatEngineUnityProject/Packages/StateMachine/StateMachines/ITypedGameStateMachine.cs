namespace RedCatEngine.StateMachine.StateMachines
{
	public interface ITypedGameStateMachine<in TBaseState> : ITypedQueueStateMachine
	{
		new ITypedQueueStateMachine Enter<TState>() where TState : class, IState, TBaseState;
		new ITypedQueueStateMachine Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>, TBaseState;
	}
}