using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using UnityEngine;

namespace RedCatEngine.Quests.Configs
{
	[CreateAssetMenu(menuName = "Configs/Quests/QuestCollectionConfig", fileName = nameof(QuestCollectionConfig))]
	public class QuestCollectionConfig : BaseConfig
	{
		public QuestConfig[] QuestConfigs;
	}
}