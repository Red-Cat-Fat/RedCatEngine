using RedCatEngine.StateMachine.StateMachines;

namespace RedCatEngine.StateMachine.Tests.SpecialSubClasses
{
	public class TestedTypedStateMachine : BaseTypedStateMachine<IExitableState>
	{
		public TestedTypedStateMachine() : base("TestedTypedStateMachine")
		{
		}

		public void AddTestState<TType>(TType state) where TType : IExitableState
			=> AddState(state);

		public IExitableState CurrenState
			=> ActiveState;
	}
}