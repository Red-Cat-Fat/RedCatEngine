using NUnit.Framework;
using RedCatEngine.StateMachine.Tests.SpecialSubClasses;

namespace RedCatEngine.StateMachine.Tests
{
	public class EnterExitStateTests : BaseStatesTests
	{
		[Test]
		public void GivenStateMachine_WhenEnterToSimpleState_ThenSwitched()
		{
			var stateAisEnter = false;
			var stateAisExit = false;
			var stateBisEnter = false;
			var stateBisExit = false;
			var stateCisEnter = false;
			var stateCisExit = false;

			_testStateA.OverrideDoEnter(() => stateAisEnter = true);
			_testStateA.OverrideDoExit(() => stateAisExit = true);
			_testStateB.OverrideDoEnter(() => stateBisEnter = true);
			_testStateB.OverrideDoExit(() => stateBisExit = true);
			_testStateC.OverrideDoEnter(() => stateCisEnter = true);
			_testStateC.OverrideDoExit(() => stateCisExit = true);

			_stateMachine.Enter<TestStateA>();
			Assert.IsTrue(_stateMachine.CurrenState is TestStateA, "Current state is not State A");
			_stateMachine.Enter<TestStateB>();
			Assert.IsFalse(_stateMachine.CurrenState is TestStateA, "Current state is old State A");
			Assert.IsTrue(_stateMachine.CurrenState is TestStateB, "Current state is not State B");
			_stateMachine.Enter<TestStateC>();
			Assert.IsFalse(_stateMachine.CurrenState is TestStateB, "Current state is old State B");
			Assert.IsTrue(_stateMachine.CurrenState is TestStateC, "Current state is not State C");

			Assert.IsTrue(stateAisEnter, "State A is not entered");
			Assert.IsTrue(stateAisExit, "State A is not exited");
			Assert.IsTrue(stateBisEnter, "State A is not entered");
			Assert.IsTrue(stateBisExit, "State A is not exited");
			Assert.IsTrue(stateCisEnter, "State A is not entered");
			Assert.IsFalse(stateCisExit, "State A is exited");
		}

		[Test]
		public void GivenStateMachine_WhenEnterToPayloadState_ThenSwitched()
		{
			var stateAisEnter = false;
			var stateAisExit = false;
			var statePayLoadIsEnter = false;
			var stateBisExit = false;
			var stateCisEnter = false;
			var stateCisExit = false;

			_testStateA.OverrideDoEnter(() => stateAisEnter = true);
			_testStateA.OverrideDoExit(() => stateAisExit = true);
			_testStatePayloadA.OverrideDoEnterPayLoad(_ => statePayLoadIsEnter = true);
			_testStatePayloadA.OverrideDoExit(() => stateBisExit = true);
			_testStateC.OverrideDoEnter(() => stateCisEnter = true);
			_testStateC.OverrideDoExit(() => stateCisExit = true);

			_stateMachine.Enter<TestStateA>();
			Assert.IsTrue(_stateMachine.CurrenState is TestStateA, "Current state is not State A");
			_stateMachine.Enter<TestStatePayloadA, TestPayLoad>(new TestPayLoad());
			Assert.IsFalse(_stateMachine.CurrenState is TestStateA, "Current state is old State A");
			Assert.IsTrue(_stateMachine.CurrenState is TestStatePayloadA, "Current state is not State B");
			_stateMachine.Enter<TestStateC>();
			Assert.IsFalse(_stateMachine.CurrenState is TestStatePayloadA, "Current state is old State B");
			Assert.IsTrue(_stateMachine.CurrenState is TestStateC, "Current state is not State C");

			Assert.IsTrue(stateAisEnter, "State A is not entered");
			Assert.IsTrue(stateAisExit, "State A is not exited");
			Assert.IsTrue(statePayLoadIsEnter, "Payload State A is not entered");
			Assert.IsTrue(stateBisExit, "Payload State A is not exited");
			Assert.IsTrue(stateCisEnter, "State A is not entered");
			Assert.IsFalse(stateCisExit, "State A is exited");
		}

		[Test]
		public void GivenStateMachine_WhenEnterToPayloadState_ThenPayloadIsCorrect()
		{
			const int valueForCheck = 42;
			var payloadWillBeCheck = false;
			_testStatePayloadA.OverrideDoEnterPayLoad(CheckCorrectGenericPayload);

			_stateMachine.Enter<TestStatePayloadA, TestPayLoad>(
				new TestPayLoad
				{
					AValue = valueForCheck,
					BValue = valueForCheck.ToString()
				});

			Assert.IsTrue(payloadWillBeCheck);

			void CheckCorrectGenericPayload(TestPayLoad payload)
			{
				Assert.AreEqual(
					payload.AValue,
					valueForCheck,
					"Not correct data from payload");
				Assert.AreEqual(
					payload.BValue,
					valueForCheck.ToString(),
					"Not correct data from payload");
				payloadWillBeCheck = true;
			}
		}
	}
}