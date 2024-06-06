using NUnit.Framework;
using RedCatEngine.StateMachine.Tests.SpecialSubClasses;

namespace RedCatEngine.StateMachine.Tests
{
	public class BaseStatesTests
	{
		protected TestedTypedStateMachine _stateMachine;
		protected TestStatePayloadA _testStatePayloadA;
		protected TestStateA _testStateA;
		protected TestStateB _testStateB;
		protected TestStateC _testStateC;

		[SetUp]
		public void SetUp()
		{
			_stateMachine = new TestedTypedStateMachine();

			_testStatePayloadA = new TestStatePayloadA();
			_testStateA = new TestStateA();
			_testStateB = new TestStateB();
			_testStateC = new TestStateC();

			_stateMachine.AddTestState<TestStateA>(_testStateA);
			_stateMachine.AddTestState<TestStateB>(_testStateB);
			_stateMachine.AddTestState<TestStateC>(_testStateC);
			_stateMachine.AddTestState<TestStatePayloadA>(_testStatePayloadA);
		}
	}
}