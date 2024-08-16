using System.Linq;
using RedCatEngine.Quests.Configs.QuestCollections;
using RedCatEngine.Quests.Mechanics.Factories;

namespace RedCatEngine.Quests.Mechanics.QuestSystems
{
	public class AchievementQuestSystem : BaseQuestSystem
	{
		private readonly SimpleQuestCollectionConfig _achievementPack;

		public AchievementQuestSystem(
			SimpleQuestCollectionConfig achievementPack,
			IQuestFactory questFactory
		)
			: base(questFactory)
		{
			_achievementPack = achievementPack;
		}

		protected sealed override void DoAfterLoadData()
		{
			foreach (var questConfig in _achievementPack.QuestConfigs)
			{
				if (ActiveQuests.Any(quest => quest.Config == questConfig))
					continue;

				var quest = QuestFactory.MakeFromConfig(questConfig);
				quest.Start(CurrentTime);
				ActiveQuests.Add(quest);
			}
		}

		protected override void DoClear()
		{
		}
	}
}