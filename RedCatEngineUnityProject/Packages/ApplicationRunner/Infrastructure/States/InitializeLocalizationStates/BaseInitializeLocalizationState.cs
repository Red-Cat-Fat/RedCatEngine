using Localization;
using RedCatEngine.StateMachine.StateMachines;
using UnityEngine;

namespace RedCatEngine.ApplicationRunner.Infrastructure.States.InitializeLocalizationStates
{
	public abstract class BaseInitializeLocalizationState : IInitializeLocalizationState
	{
		private readonly ITypedQueueStateMachine _gameStateMachine;

		protected BaseInitializeLocalizationState(ITypedQueueStateMachine gameStateMachine)
		{
			_gameStateMachine = gameStateMachine;
		}
		
		public abstract string GetLanguage();
		
		public void Enter()
		{
			var currentLanguage = GetLanguage();
			switch (currentLanguage)
			{
				case "ru":
					LocalizeSystem.Init(SystemLanguage.Russian);
					break;
				case "en":
					LocalizeSystem.Init(SystemLanguage.English);
					break;
				case "tr":
					LocalizeSystem.Init(SystemLanguage.Turkish);
					break;
				case "fr":
					LocalizeSystem.Init(SystemLanguage.French);
					break;
				case "de":
					LocalizeSystem.Init(SystemLanguage.German);
					break;
				default:
					LocalizeSystem.Init(SystemLanguage.English);
					break;
			}
			_gameStateMachine.EnterNextFromQueue();
		}

		public void Exit()
		{
			
		}
	}
}