#if YANDEX_GAME
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.StateMachine.StateMachines;
using YG;

namespace RedCatEngine.ApplicationRunner.Infrastructure.States.InitializeLocalizationStates
{
	public class YandexInitializeLocalizationState : BaseInitializeLocalizationState
	{
		[Inject]
		public YandexInitializeLocalizationState(ITypedQueueStateMachine gameStateMachine)
			: base(gameStateMachine) { }

		public override string GetLanguage()
			=> YandexGame.EnvironmentData.language;
	}
}

#endif