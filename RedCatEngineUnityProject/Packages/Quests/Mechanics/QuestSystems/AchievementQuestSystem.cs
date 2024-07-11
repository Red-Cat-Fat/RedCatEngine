using System.Linq;
using RedCatEngine.Quests.Configs;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Factories;

namespace RedCatEngine.Quests.Mechanics.QuestSystems
{
	public class AchievementQuestSystem : BaseQuestSystem
	{
		private readonly QuestCollectionConfig _achievementPack;

		public AchievementQuestSystem(
			QuestsDataContainer data,
			QuestCollectionConfig achievementPack,
			IQuestFactory questFactory
		)
			: base(questFactory)
		{
			_achievementPack = achievementPack;
			LoadData(data);
			DoAfterLoadData();
		}

		private void DoAfterLoadData()
		{
			var activeQuests = GetActiveQuest();
			foreach (var questConfig in _achievementPack.QuestConfigs)
			{
				if (activeQuests.Any(quest => quest.Config == questConfig))
					continue;

				_activeQuest.Add(_questFactory.MakeFromConfig(questConfig));
			}
		}
	}
}