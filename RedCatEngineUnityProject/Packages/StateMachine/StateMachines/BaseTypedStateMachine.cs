using System;
using System.Collections.Generic;
using RedCatEngine.StateMachine.Exceptions;

namespace RedCatEngine.StateMachine.StateMachines
{
	public abstract class BaseTypedStateMachine : ITypedGameStateMachine
	{
		private readonly Dictionary<Type, IExitableState> _states = new();
		private readonly Queue<StateStepData> _queue = new();
		private readonly List<StateStepData> _stepHistory = new();

		protected IExitableState ActiveState { get; private set; }

		protected void AddState<TType>(IExitableState state) where TType : IExitableState
		{
			if (_states.ContainsKey(typeof(TType)))
				throw new AlreadyContainStateException(typeof(TType));
			_states.Add(typeof(TType), state);
		}

		protected void AddState<TType>(TType state) where TType : IExitableState
		{
			if (_states.ContainsKey(typeof(TType)))
				throw new AlreadyContainStateException(typeof(TType));
			_states.Add(typeof(TType), state);
		}

		public ITypedQueueStateMachine Enter<TState>() where TState : class, IState
		{
			IState state = SelectStateAsActive<TState>();
			state.Enter();
			_stepHistory.Add(new StateStepData(typeof(TState)));
			return this;
		}

		public ITypedQueueStateMachine Enter<TState, TPayload>(TPayload payload)
			where TState : class, IPayloadedState<TPayload>
		{
			var state = SelectStateAsActive<TState>();
			state.Enter(payload);
			_stepHistory.Add(
				new StateStepData(
					typeof(TState),
					typeof(TPayload),
					payload));
			return this;
		}


		public ITypedQueueStateMachine EnterNextFromQueue()
		{
			var data = _queue.Dequeue();
			var nextState = SelectStateAsActive(data.StateType);
			if (!data.IsPayLoadState)
				(nextState as IState)?.Enter();
			else
				(nextState as IPayloadedState)?.Enter(data.Payload);

			_stepHistory.Add(data);
			return this;
		}

		public ITypedQueueStateMachine AddToQueue<TState>() where TState : class, IState
		{
			_queue.Enqueue(new StateStepData(typeof(TState)));
			return this;
		}

		public ITypedQueueStateMachine AddToQueue<TState, TPayload>(TPayload payload)
			where TState : class, IPayloadedState<TPayload>
		{
			_queue.Enqueue(
				new StateStepData(
					typeof(TState),
					typeof(TPayload),
					payload));
			return this;
		}

		private TState SelectStateAsActive<TState>() where TState : class, IExitableState
		{
			ActiveState?.Exit();

			var state = GetState<TState>();
			ActiveState = state;

			return state;
		}

		private IExitableState SelectStateAsActive(Type type)
		{
			ActiveState?.Exit();

			var state = GetState(type);
			ActiveState = state;

			return state;
		}

		private TState GetState<TState>() where TState : class, IExitableState
			=> GetState(typeof(TState)) as TState;

		private IExitableState GetState(Type type)
		{
			if (!_states.TryGetValue(type, out var targetState))
				throw new NotFoundStateException(type);
			return targetState;
		}
	}
}