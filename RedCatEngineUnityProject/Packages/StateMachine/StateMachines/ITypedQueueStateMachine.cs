namespace RedCatEngine.StateMachine.StateMachines
{
	public interface ITypedQueueStateMachine
	{
		ITypedQueueStateMachine AddToQueue<TState>() where TState : class, IState;
		ITypedQueueStateMachine AddToQueue<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
		ITypedQueueStateMachine EnterNextFromQueue();
	}
}