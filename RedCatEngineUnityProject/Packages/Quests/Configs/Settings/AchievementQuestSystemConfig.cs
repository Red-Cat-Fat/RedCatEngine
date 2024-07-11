using JetBrains.Annotations;
using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.QuestGenerators;
using RedCatEngine.Quests.Mechanics.QuestSystems;
using UnityEngine;

namespace RedCatEngine.Quests.Configs.Settings
{
	[CreateAssetMenu(menuName = "Configs/Quests/AchievementQuestSystemConfig", fileName = nameof(AchievementQuestSystemConfig))]
	public class AchievementQuestSystemConfig : BaseConfig
	{
		public QuestCollectionConfig AchievementPack;

		[NotNull]
		public AchievementQuestSystem Make(IApplicationContainer applicationContainer, QuestsDataContainer data)
		{
			var questFactory
				= new CollectionSelectorQuestFactory(
					applicationContainer,
					new QueueQuestSelector(AchievementPack));

			return new AchievementQuestSystem(
				data,
				AchievementPack,
				questFactory);
		}
	}
}