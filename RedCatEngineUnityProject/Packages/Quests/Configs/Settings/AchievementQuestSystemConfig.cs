using JetBrains.Annotations;
using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.Quests.Configs.QuestCollections;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.QuestGenerators;
using RedCatEngine.Quests.Mechanics.QuestSystems;
using UnityEngine;

namespace RedCatEngine.Quests.Configs.Settings
{
	[CreateAssetMenu(menuName = "Configs/Quests/AchievementQuestSystemConfig", fileName = nameof(AchievementQuestSystemConfig))]
	public class AchievementQuestSystemConfig : BaseConfig
	{
		public SimpleQuestCollectionConfig AchievementPack;

		[NotNull]
		public AchievementQuestSystem Make(IApplicationContainer applicationContainer)
		{
			var questFactory
				= new CollectionSelectorQuestFactory(
					applicationContainer,
					new QueueQuestSelector(AchievementPack));

			return new AchievementQuestSystem(
				AchievementPack,
				questFactory);
		}
	}
}