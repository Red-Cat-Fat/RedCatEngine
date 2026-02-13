using System;
using System.Collections.Generic;
using RedCatEngine.StateMachine.Exceptions;
using UnityEngine;

namespace RedCatEngine.StateMachine.StateMachines
{
	public abstract class BaseTypedStateMachine<TBaseState> : ITypedGameStateMachine<TBaseState>
		where TBaseState : IExitableState
	{
		private readonly string _name;
		protected readonly Dictionary<Type, TBaseState> _states = new();
		private readonly Queue<StateStepData> _queue = new();
		private readonly List<StateStepData> _stepHistory = new();

		protected BaseTypedStateMachine(string name)
		{
			_name = name;
		}
		
		protected TBaseState ActiveState { get; private set; }

		protected void AddState<TType>(TBaseState state) where TType : TBaseState
		{
			if (_states.ContainsKey(typeof(TType)))
				throw new AlreadyContainStateException(typeof(TType));
			_states.Add(typeof(TType), state);
		}

		protected void AddState<TState>(TState state) where TState : TBaseState
		{
			if (_states.ContainsKey(typeof(TState)))
				throw new AlreadyContainStateException(typeof(TState));
			_states.Add(typeof(TState), state);
		}

		protected void AddState(Type stateType, TBaseState state)
		{
			if (!_states.TryAdd(stateType, state))
				throw new AlreadyContainStateException(stateType);
		}

		public ITypedQueueStateMachine Enter<TState>() where TState : class, TBaseState, IState
		{
			var state = SelectStateAsActive<TState>();
			AddToHistory(new StateStepData(typeof(TState)));
			EnterState(state);
			return this;
		}

		private void EnterState(IState state)
		{
			if(state == null) 
				throw new ArgumentNullException(string.Format("State {0} is null", nameof(state)));
			state.Enter();
		}

		private void EnterState<TPayload>(IPayloadedState state, TPayload payload)
		{
			if(state == null) 
				throw new ArgumentNullException(string.Format("State {0} is null", nameof(state)));
			state.Enter(payload);
		}

		public ITypedQueueStateMachine Enter<TState, TPayload>(TPayload payload)
			where TState : class, TBaseState, IPayloadedState<TPayload>
		{
			var state = SelectStateAsActive<TState>();
			AddToHistory(
				new StateStepData(
					typeof(TState),
					typeof(TPayload),
					payload));
			state.Enter(payload);
			return this;
		}


		public ITypedQueueStateMachine EnterNextFromQueue()
		{
			if (_queue.Count == 0)
				throw new QueueStateIsEmptyException(_name);

			var data = _queue.Dequeue();
			AddToHistory(data);
			var nextState = SelectStateAsActive(data.StateType);
			if (!data.IsPayLoadState)
				EnterState(nextState as IState);
			else
				EnterState(nextState as IPayloadedState, data.Payload);

			return this;
		}

		public ITypedQueueStateMachine AddToQueue<TState>() where TState : class, IState
		{
			return AddToQueue(new StateStepData(typeof(TState)));
		}

		public ITypedQueueStateMachine AddToQueue<TState, TPayload>(TPayload payload)
			where TState : class, IPayloadedState<TPayload>
		{
			return AddToQueue(
				new StateStepData(
					typeof(TState),
					typeof(TPayload),
					payload));
		}

		private void AddToHistory(StateStepData data)
		{
			if(!data.IsPayLoadState)
				Debug.LogFormat("[{0}] Enter to state {1}", _name, data.StateType);
			else
				Debug.LogFormat("[{0}] Enter to payload state {1} with {2}", _name, data.StateType, data.Payload);
			_stepHistory.Add(data);
		}

		private ITypedQueueStateMachine AddToQueue(StateStepData data)
		{
			Debug.LogFormat("[{0}] Add to queue state {1}", _name, data.StateType);
			_queue.Enqueue(data);
			return this;
		}

		private TState SelectStateAsActive<TState>() where TState : class, TBaseState
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

		protected virtual TBaseState GetState(Type type)
		{
			if (!_states.TryGetValue(type, out var targetState))
				throw new NotFoundStateException(type);
			if(targetState == null) 
				Debug.LogError("State is null");
			return targetState;
		}
	}
}
