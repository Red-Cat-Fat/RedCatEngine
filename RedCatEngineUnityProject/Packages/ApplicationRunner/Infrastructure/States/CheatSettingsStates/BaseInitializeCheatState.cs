using RedCatEngine.StateMachine.StateMachines;
using UnityEngine;

namespace RedCatEngine.ApplicationRunner.Infrastructure.States.CheatSettingsStates
{
	public abstract class BaseInitializeCheatState : IInitializeCheatState
	{
		private readonly TypeBasedStateMachine<IState> _gameStateMachine;
		private readonly IInitializeCheatPayload _cheatsPayload;

		protected BaseInitializeCheatState(
			TypeBasedStateMachine<IState> gameStateMachine,
			IInitializeCheatPayload cheatsPayload
		)
		{
			_gameStateMachine = gameStateMachine;
			_cheatsPayload = cheatsPayload;
		}

		public void Exit() { }

		public void Enter()
		{
			Debug.Log("Enter to SpawnCheatConsoleState");
			Object.Instantiate(_cheatsPayload.ConsolePrefab);
			
			_gameStateMachine
				.EnterNextFromQueue();
		}
		
		protected abstract void AttachCheats();
	}
}