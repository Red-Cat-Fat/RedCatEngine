using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Mechanics.QuestSystems
{
	public class StoryQuestSystem : BaseQuestSystem
	{
		[Inject]
		public StoryQuestSystem(StoryQuestFactory questQuestFactory) : base(questQuestFactory)
		{
		}

		protected override void DoAfterLoadData()
		{
		}

		public void StartQuest(QuestConfig quest)
		{
			var newQuest = CreateAndStartNewQuest(quest);
			newQuest.ChangeQuestStateEvent += OnChangeQuestState;
			AddActiveQuest(newQuest);
		}

		public void FinishedQuest(QuestConfig quest)
		{
			IQuest questToFinish = null;
			foreach (var activeQuest in ActiveQuests)
			{
				if (activeQuest.Config != quest)
					continue;

				questToFinish = activeQuest;
				break;
			}

			questToFinish?.SuccessFinished();
		}

		protected override void DoClear()
		{
			foreach (var quest in ActiveQuests)
				quest.ChangeQuestStateEvent -= OnChangeQuestState;
		}
	}
}