using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.Quests.Mechanics;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.QuestGenerators;
using RedCatEngine.Quests.Mechanics.QuestSystems;

namespace RedCatEngine.Quests.Configs.Settings
{
	public class DailyQuestSystemConfig : BaseConfig
	{
		public QuestCollectionConfig DailyQuestPack;
		public int QuestPerDay = 3;
		public int DailyTimeLiveSeconds = 60 * 60 * 24;

		public DailyQuestSystem Make(IApplicationContainer applicationContainer, DailyQuestsData data)
		{
			var questFactory
				= new CollectionSelectorQuestFactory(
					applicationContainer,
					new RandomQuestSelector(DailyQuestPack));

			return new DailyQuestSystem(
				data,
				questFactory,
				QuestPerDay,
				DailyTimeLiveSeconds);
		}
	}
}