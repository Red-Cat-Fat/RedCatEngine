using System;
using NUnit.Framework;
using RedCatEngine.StateMachine.Exceptions;
using RedCatEngine.StateMachine.Tests.SpecialSubClasses;

namespace RedCatEngine.StateMachine.Tests
{
	public class ExceptionStateMachineTests : BaseStatesTests
	{
		[Test]
		public void GivenStateMachine_WhenAddDuplicateClass_ThenCatchAlreadyContainStateException()
		{
			try
			{
				_stateMachine.AddTestState<TestStateA>(new TestStateA());
			}
			catch (Exception exception)
			{
				Assert.IsTrue(
					exception is AlreadyContainStateException,
					"Catch incorrect error");
				Assert.IsTrue(
					((AlreadyContainStateException)exception).IncorrectType == typeof(TestStateA),
					"Invalid class specified");
				return;
			}

			Assert.IsTrue(false, "Not catch exception");
		}
		
		[Test]
		public void GivenStateMachine_WhenEnterUnknownState_ThenCatchNotFoundStateException()
		{
			try
			{
				_stateMachine.Enter<UnknownState>();
			}
			catch (Exception exception)
			{
				Assert.IsTrue(
					exception is NotFoundStateException,
					"Catch incorrect error");
				Assert.IsTrue(
					((NotFoundStateException)exception).NotFoundState == typeof(UnknownState),
					"Invalid class specified");
				return;
			}

			Assert.IsTrue(false, "Not catch exception");
		}
	}
}