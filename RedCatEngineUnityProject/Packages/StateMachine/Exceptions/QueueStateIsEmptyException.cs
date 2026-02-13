using System;

namespace RedCatEngine.StateMachine.Exceptions
{
	public class QueueStateIsEmptyException : Exception
	{
		public string StateMachineName;

		public QueueStateIsEmptyException(string stateMachineName)
			: base(string.Format("Queue is empty in state machine {0}", stateMachineName))
		{
			StateMachineName = stateMachineName;
		}
	}
}
