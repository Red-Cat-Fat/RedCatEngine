using System;

namespace RedCatEngine.StateMachine.Exceptions
{
	public class AlreadyContainStateException : Exception
	{
		public Type IncorrectType;
		public AlreadyContainStateException(Type type)
			: base(string.Format("This type {0} class is already contained in this state machine", type))
		{
			IncorrectType = type;
		}
	}
}