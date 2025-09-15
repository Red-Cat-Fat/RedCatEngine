using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.StateMachine.StateMachines;

namespace RedCatEngine.ApplicationRunner.Infrastructure.States.InitializeLocalizationStates
{
	public class EditorLocalizationState : BaseInitializeLocalizationState
	{
		private readonly string _language;

		[Inject]
		public EditorLocalizationState(ITypedQueueStateMachine gameStateMachine, string language = "ru") : base(gameStateMachine)
		{
			_language = language;
		}

		public override string GetLanguage()
			=> _language;
	}
}