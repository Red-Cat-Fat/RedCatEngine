using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.QuestGenerators;
using RedCatEngine.Quests.Mechanics.QuestSystems;
using UnityEngine;

namespace RedCatEngine.Quests.Configs.Settings
{
	[CreateAssetMenu(menuName = "Configs/Quests/DailyQuestSystemConfig", fileName = nameof(DailyQuestSystemConfig))]
	public class DailyQuestSystemConfig : BaseConfig
	{
		public QuestCollectionConfig DailyQuestPack;
		public int QuestPerDay = 3;
		public int DailyTimeLiveSeconds = 60 * 60 * 24;

		public DailyQuestSystem Make(IApplicationContainer applicationContainer, QuestsDataContainer data)
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