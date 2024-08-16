using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.Quests.Configs.QuestCollections;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.QuestGenerators;
using RedCatEngine.Quests.Mechanics.QuestSystems;
using UnityEngine;

namespace RedCatEngine.Quests.Configs.Settings
{
	[CreateAssetMenu(menuName = "Configs/Quests/DailyQuestSystemConfig", fileName = nameof(DailyQuestSystemConfig))]
	public class DailyQuestSystemConfig : BaseConfig
	{
		public SimpleQuestCollectionConfig DailySimpleQuestPack;
		public int QuestPerDay = 3;
		public int DailyTimeLiveSeconds = 60 * 60 * 24;

		public DailyQuestSystem Make(IApplicationContainer applicationContainer)
		{
			var questFactory
				= new CollectionSelectorQuestFactory(
					applicationContainer,
					new RandomQuestSelector(DailySimpleQuestPack));

			return new DailyQuestSystem(
				questFactory,
				QuestPerDay,
				DailyTimeLiveSeconds);
		}
	}
}