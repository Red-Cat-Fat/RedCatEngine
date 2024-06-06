namespace RedCatEngine.StateMachine.StateMachines
{
	public interface ITypedGameStateMachine : ITypedQueueStateMachine
	{
		ITypedQueueStateMachine Enter<TState>() where TState : class, IState;
		ITypedQueueStateMachine Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
	}
}