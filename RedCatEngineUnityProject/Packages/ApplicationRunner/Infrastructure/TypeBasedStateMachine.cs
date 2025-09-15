using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;
using RedCatEngine.StateMachine.StateMachines;
using UnityEngine;

namespace RedCatEngine.ApplicationRunner.Infrastructure
{
	public abstract class TypeBasedStateMachine<TBaseState> : BaseTypedStateMachine<TBaseState> 
		where TBaseState : class, IExitableState
	{
		private readonly ICreator _creator;

		protected TypeBasedStateMachine(ICreator creator, string name) : base(name)
		{
			_creator = creator;
		}

		protected void AddState<TTagState, TInstanceState>()
			where TTagState : TBaseState
			where TInstanceState : TTagState
			=> AddState<TTagState>(_creator.Create<TInstanceState>());

		protected void AddState<TTagState, TInstanceState>(params object[] context)
			where TTagState : TBaseState
			where TInstanceState : TTagState
			=> AddState<TTagState>(_creator.Create<TInstanceState>(context));

		protected void AddState<TInstanceState>()
			where TInstanceState : TBaseState
			=> AddState<TInstanceState, TInstanceState>();

		protected override TBaseState GetState(Type type)
		{
			if (_states.TryGetValue(type, out var targetState)) 
				return targetState;

			var newState = _creator.Create(type);
			targetState = newState as TBaseState;
			AddState(type, targetState);
			if(targetState == null) 
				Debug.LogError("State is null");
			return targetState;
		}
	}
}