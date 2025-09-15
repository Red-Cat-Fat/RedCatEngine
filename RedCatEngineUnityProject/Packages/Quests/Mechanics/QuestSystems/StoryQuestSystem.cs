using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Factories;

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
			ActiveQuests.Add(newQuest);
		}

		public void FinishedQuest(QuestConfig quest)
		{
			foreach (var activeQuest in ActiveQuests)
				if (activeQuest.Config == quest)
					activeQuest.SuccessFinished();
		}

		protected override void DoClear()
		{
			foreach (var quest in ActiveQuests)
				quest.ChangeQuestStateEvent -= OnChangeQuestState;
		}
	}
}