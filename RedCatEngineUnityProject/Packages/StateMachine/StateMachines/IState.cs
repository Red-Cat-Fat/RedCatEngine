namespace RedCatEngine.StateMachine.StateMachines
{
	public interface IState : IExitableState
	{
		void Enter();
	}
}