using System;
using RedCatEngine.StateMachine.StateMachines;
using UnityEngine;

namespace RedCatEngine.StateMachine.Tests.SpecialSubClasses
{
	public class BasePayloadLoggerTestState<TPayload> : IPayloadedState<TPayload>
	{
		private readonly string _name;
		private Action _doExit;
		private Action<TPayload> _doEnterPayload;

		protected BasePayloadLoggerTestState(
			string name,
			Action doExit = null,
			Action<TPayload> doEnterPayload = null
		)
		{
			_name = name;
			_doExit = doExit;
			_doEnterPayload = doEnterPayload;
		}

		public void Enter(object payload)
		{
			Debug.Log($"{_name} enter with payload: {payload}");
			Enter((TPayload)payload);
		}

		public void Enter(TPayload payload)
		{
			Debug.Log($"{_name} enter<TPayload> with payload: {payload}");
			_doEnterPayload?.Invoke(payload);
		}

		public void OverrideDoEnterPayLoad(Action<TPayload> newDoEnterPayload)
			=> _doEnterPayload = newDoEnterPayload;

		public void OverrideDoExit(Action newDoExit)
			=> _doExit = newDoExit;

		public void Exit()
		{
			Debug.Log(_name + " exit.");
			_doExit?.Invoke();
		}
	}
}