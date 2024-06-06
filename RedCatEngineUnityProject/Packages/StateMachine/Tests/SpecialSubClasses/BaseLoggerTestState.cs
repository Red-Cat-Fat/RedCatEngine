using System;
using RedCatEngine.StateMachine.StateMachines;
using UnityEngine;

namespace RedCatEngine.StateMachine.Tests.SpecialSubClasses
{
	public class BaseLoggerTestState : IState
	{
		private readonly string _name;
		private Action _doEnter;
		private Action _doExit;

		protected BaseLoggerTestState(string name, Action doEnter = null, Action doExit = null)
		{
			_name = name;
			_doEnter = doEnter;
			_doExit = doExit;
		}

		public void Enter()
		{
			Debug.Log(_name + " enter.");
			_doEnter?.Invoke();
		}

		public void Exit()
		{
			Debug.Log(_name + " exit.");
			_doExit?.Invoke();
		}
		
		public void OverrideDoEnter(Action newDoEnter)
		{
			_doEnter = newDoEnter;
		}

		public void OverrideDoExit(Action newDoExit)
		{
			_doExit = newDoExit;
		}
	}
}