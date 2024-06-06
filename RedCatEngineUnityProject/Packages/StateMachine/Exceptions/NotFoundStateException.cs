using System;

namespace RedCatEngine.StateMachine.Exceptions
{
	public class NotFoundStateException : Exception
	{
		public Type NotFoundState;
		public NotFoundStateException(Type type)
			: base(string.Format("This type {0} class not found in this state machine", type))
		{
			NotFoundState = type;
		}
	}
}