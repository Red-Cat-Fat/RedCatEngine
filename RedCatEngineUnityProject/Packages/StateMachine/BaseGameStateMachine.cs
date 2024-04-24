using System;
using System.Collections.Generic;

namespace RedCatEngine.StateMachine
{
	public abstract class BaseGameStateMachine : IGameStateMachine
	{
		private readonly Dictionary<Type, IExitableState> _states = new();
		private IExitableState _activeState;

		protected void AddState<TType>(IExitableState state)
			=> _states.Add(typeof(TType), state);

		public void Enter<TState>() where TState : class, IState
		{
			IState state = SelectStateAsActive<TState>();
			state.Enter();
		}

		public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
		{
			var state = SelectStateAsActive<TState>();
			state.Enter(payload);
		}

		private TState SelectStateAsActive<TState>() where TState : class, IExitableState
		{
			_activeState?.Exit();

			var state = GetState<TState>();
			_activeState = state;

			return state;
		}

		private TState GetState<TState>() where TState : class, IExitableState
			=> _states[typeof(TState)] as TState;
	}
}