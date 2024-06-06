using NUnit.Framework;
using RedCatEngine.StateMachine.Tests.SpecialSubClasses;

namespace RedCatEngine.StateMachine.Tests
{
	public class QueueStateMachineTests : BaseStatesTests
	{
		[Test]
		public void GivenSimpleQueueStates_WhenTrySwitch_ThenSwitched()
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

			_stateMachine
				.AddToQueue<TestStateA>()
				.AddToQueue<TestStateB>()
				.AddToQueue<TestStateC>();

			_stateMachine.EnterNextFromQueue();
			Assert.IsTrue(_stateMachine.CurrenState is TestStateA, "Current state is not State A");
			_stateMachine.EnterNextFromQueue();
			Assert.IsFalse(_stateMachine.CurrenState is TestStateA, "Current state is old State A");
			Assert.IsTrue(_stateMachine.CurrenState is TestStateB, "Current state is not State B");
			_stateMachine.EnterNextFromQueue();
			Assert.IsFalse(_stateMachine.CurrenState is TestStateB, "Current state is old State B");
			Assert.IsTrue(_stateMachine.CurrenState is TestStateC, "Current state is not State C");

			Assert.IsTrue(stateAisEnter, "State A is not entered");
			Assert.IsTrue(stateAisExit, "State A is not exited");
			Assert.IsTrue(stateBisEnter, "State B is not entered");
			Assert.IsTrue(stateBisExit, "State B is not exited");
			Assert.IsTrue(stateCisEnter, "State C is not entered");
			Assert.IsFalse(stateCisExit, "State C is exited");
		}

		[Test]
		public void GivenSimpleQueueStateAfterEnter_WhenEnterFromNextQueue_ThenSwitchedCorrected()
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

			_stateMachine
				.Enter<TestStateA>()
				.AddToQueue<TestStateB>()
				.AddToQueue<TestStateC>();

			Assert.IsTrue(_stateMachine.CurrenState is TestStateA, "Current state is not State A");
			_stateMachine.EnterNextFromQueue();
			Assert.IsFalse(_stateMachine.CurrenState is TestStateA, "Current state is old State A");
			Assert.IsTrue(_stateMachine.CurrenState is TestStateB, "Current state is not State B");
			_stateMachine.EnterNextFromQueue();
			Assert.IsFalse(_stateMachine.CurrenState is TestStateB, "Current state is old State B");
			Assert.IsTrue(_stateMachine.CurrenState is TestStateC, "Current state is not State C");

			Assert.IsTrue(stateAisEnter, "State A is not entered");
			Assert.IsTrue(stateAisExit, "State A is not exited");
			Assert.IsTrue(stateBisEnter, "State B is not entered");
			Assert.IsTrue(stateBisExit, "State B is not exited");
			Assert.IsTrue(stateCisEnter, "State C is not entered");
			Assert.IsFalse(stateCisExit, "State C is exited");
		}
		
		[Test]
		public void GivenPayloadQueueStateAfterEnter_WhenEnterFromNextQueue_ThenSwitchedCorrected()
		{
			const int valueForCheck = 42;
			var payloadWillBeCheck = false;
			var stateAisEnter = false;
			var stateAisExit = false;

			_testStateA.OverrideDoEnter(() => stateAisEnter = true);
			_testStateA.OverrideDoExit(() => stateAisExit = true);
			_testStatePayloadA.OverrideDoEnterPayLoad(CheckCorrectPayload);

			_stateMachine
				.Enter<TestStateA>()
				.AddToQueue<TestStatePayloadA, TestPayLoad>(
					new TestPayLoad
					{
						AValue = valueForCheck,
						BValue = valueForCheck.ToString()
					});

			Assert.IsTrue(_stateMachine.CurrenState is TestStateA, "Current state is not State A");
			_stateMachine.EnterNextFromQueue();
			Assert.IsFalse(_stateMachine.CurrenState is TestStateA, "Current state is old State A");
			Assert.IsTrue(
				_stateMachine.CurrenState is TestStatePayloadA,
				"Current state is not State TestStatePayloadA");
			
			Assert.IsTrue(stateAisEnter, "State A is not entered");
			Assert.IsTrue(stateAisExit, "State A is not exited");
			Assert.IsTrue(payloadWillBeCheck, "Not checked object payload");

			void CheckCorrectPayload(TestPayLoad payload)
			{
				Assert.NotNull(payload, "Payload is incorrect type");
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

		[Test]
		public void GivenSimpleQueueStatesAndPayloadQueueStates_WhenAlternatingSwitchStateTypes_ThenSwitchedCorrected()
		{
			const int valueForCheck = 42;
			var payloadCheckCount = 0;

			var stateAisEnter = false;
			var stateAisExit = false;
			var stateBisEnter = false;
			var stateBisExit = false;
			var stateCisEnter = false;
			var stateCisExit = false;
			var statePayloadAisExit = false;

			_testStateA.OverrideDoEnter(() => stateAisEnter = true);
			_testStateA.OverrideDoExit(() => stateAisExit = true);
			_testStateB.OverrideDoEnter(() => stateBisEnter = true);
			_testStateB.OverrideDoExit(() => stateBisExit = true);
			_testStateC.OverrideDoEnter(() => stateCisEnter = true);
			_testStateC.OverrideDoExit(() => stateCisExit = true);
			_testStatePayloadA.OverrideDoEnterPayLoad(CheckCorrectPayload);
			_testStatePayloadA.OverrideDoExit(() => statePayloadAisExit = true);

			var payloadDataFirst = new TestPayLoad
			{
				AValue = valueForCheck,
				BValue = valueForCheck.ToString()
			};
			var payloadDataSecond = new TestPayLoad
			{
				AValue = valueForCheck + 1,
				BValue = (valueForCheck + 1).ToString()
			};
			_stateMachine
				.Enter<TestStateA>()
				.AddToQueue<TestStatePayloadA, TestPayLoad>(payloadDataFirst)
				.AddToQueue<TestStateB>()
				.AddToQueue<TestStatePayloadA, TestPayLoad>(payloadDataSecond)
				.AddToQueue<TestStateC>();

			Assert.IsTrue(_stateMachine.CurrenState is TestStateA, "Current state is not State A");

			_stateMachine.EnterNextFromQueue();
			Assert.IsFalse(_stateMachine.CurrenState is TestStateA, "Current state is old State A");
			Assert.IsTrue(
				_stateMachine.CurrenState is TestStatePayloadA,
				"Current state is not State Test State Payload A");

			_stateMachine.EnterNextFromQueue();
			Assert.IsFalse(_stateMachine.CurrenState is TestStatePayloadA, "Current state is old Test State Payload A");
			Assert.IsTrue(_stateMachine.CurrenState is TestStateB, "Current state is not State B");

			_stateMachine.EnterNextFromQueue();
			Assert.IsFalse(_stateMachine.CurrenState is TestStateB, "Current state is old State B");
			Assert.IsTrue(
				_stateMachine.CurrenState is TestStatePayloadA,
				"Current state is not State Test State Payload A");

			_stateMachine.EnterNextFromQueue();
			Assert.IsFalse(_stateMachine.CurrenState is TestStatePayloadA, "Current state is old Test State Payload A");
			Assert.IsTrue(_stateMachine.CurrenState is TestStateC, "Current state is not State C");

			Assert.IsTrue(stateAisEnter, "State A is not entered");
			Assert.IsTrue(stateAisExit, "State A is not exited");
			Assert.AreEqual(
				payloadCheckCount,
				2,
				"Not checked object payload");
			Assert.IsTrue(statePayloadAisExit, "Payload State A is exited");
			Assert.IsTrue(stateBisEnter, "State B is not entered");
			Assert.IsTrue(stateBisExit, "State B is not exited");
			Assert.IsTrue(stateCisEnter, "State C is not entered");
			Assert.IsFalse(stateCisExit, "State C is exited");

			void CheckCorrectPayload(object payloadObj)
			{
				var payload = payloadObj as TestPayLoad;
				Assert.NotNull(payload, "Payload is incorrect type");
				Assert.AreEqual(
					payload.AValue,
					valueForCheck + payloadCheckCount,
					$"Not correct data from payload on {(payloadCheckCount == 0 ? "first" : "second")} check");
				Assert.AreEqual(
					payload.BValue,
					(valueForCheck + payloadCheckCount).ToString(),
					$"Not correct data from payload on {(payloadCheckCount == 0 ? "first" : "second")} check");
				payloadCheckCount++;
			}
		}

	}
}