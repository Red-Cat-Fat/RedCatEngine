using System;
using System.Linq;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using UnityEngine;

namespace RedCatEngine.Quests.Configs.QuestCollections
{
	[CreateAssetMenu(
		menuName = "Configs/Quests/QuestCollection/QuestCollectionConfig",
		fileName = nameof(SimpleQuestCollectionConfig))]
	public class SimpleQuestCollectionConfig : BaseConfig, IQuestCollection
	{
		[Tooltip("Quest for create")]
		public QuestConfig[] QuestConfigs;

		[Tooltip("Quest for load, but not create from this array")]
		public QuestConfig[] HiddenQuestConfig;

		public int Count
			=> QuestConfigs.Length;

		public QuestConfig this[int index]
			=> QuestConfigs[index % QuestConfigs.Length];

		public bool TryFindById(ConfigID<QuestConfig> questId, out QuestConfig questConfig)
			=> QuestConfigs.TryFindById(questId, out questConfig) ||
			   QuestConfigs
				   .SelectMany(
					   quest => quest is IQuestRedirected questRedirected
						   ? questRedirected.GetAllQuestVariantForLoad()
						   : Array.Empty<QuestConfig>())
				   .TryFindById(questId, out questConfig) ||
			   HiddenQuestConfig.TryFindById(questId, out questConfig);

		public string GetName()
			=> name;
	}
}