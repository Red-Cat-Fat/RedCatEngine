using System;
using RedCatEngine.StateMachine.StateMachines;

namespace RedCatEngine.StateMachine.Tests.SpecialSubClasses
{
	public class TestPayLoad
	{
		public int AValue;
		public string BValue;

		public override string ToString()
		{
			return $"TestPayLoad: [ AValue: {AValue}; BValue: {BValue}]";
		}
	}

	public class TestStatePayloadA : BasePayloadLoggerTestState<TestPayLoad>
	{
		public TestStatePayloadA()
			: base("State Payload A") { }
	}

	public class TestStateA : BaseLoggerTestState
	{
		public TestStateA()
			: base("State A") { }
	}

	public class TestStateB : BaseLoggerTestState
	{
		public TestStateB()
			: base("State B") { }
	}

	public class TestStateC : BaseLoggerTestState
	{
		public TestStateC()
			: base("State C") { }
	}
	
	class UnknownState :  IState
	{
		public void Exit()
			=> throw new NotImplementedException();

		public void Enter()
			=> throw new NotImplementedException();
	}
}