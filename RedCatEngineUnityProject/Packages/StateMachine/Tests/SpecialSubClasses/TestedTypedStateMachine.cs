using RedCatEngine.StateMachine.StateMachines;

namespace RedCatEngine.StateMachine.Tests.SpecialSubClasses
{
	public class TestedTypedStateMachine : BaseTypedStateMachine
	{
		public void AddTestState<TType>(IExitableState state) where TType : IExitableState
			=> AddState<TType>(state);

		public IExitableState CurrenState
			=> ActiveState;
	}
}