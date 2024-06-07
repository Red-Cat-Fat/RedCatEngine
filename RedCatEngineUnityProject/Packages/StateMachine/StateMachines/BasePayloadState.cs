namespace RedCatEngine.StateMachine.StateMachines
{
	public abstract class BasePayloadState<TPayload> : IPayloadedState<TPayload>
	{
		public void Enter(object payload)
		{
			Enter((TPayload)payload);
		}

		public abstract void Exit();
		public abstract void Enter(TPayload payload);
	}
}