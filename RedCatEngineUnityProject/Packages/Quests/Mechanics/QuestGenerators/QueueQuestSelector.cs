using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs;
using RedCatEngine.Quests.Configs.Quests;

namespace RedCatEngine.Quests.Mechanics.QuestGenerators
{
	public class QueueQuestSelector : IQuestSelector
	{
		private readonly QuestCollectionConfig _achievementPack;

		public string Name
			=> string.Format("QueueQuestSelector from {0}", _achievementPack.name);

		public int TotalQuestVariants
			=> _achievementPack.QuestConfigs.Length;

		private int _index = 0;

		public QueueQuestSelector(QuestCollectionConfig achievementPack)
			=> _achievementPack = achievementPack;

		public bool TryGetNextQuest(out QuestConfig questConfig)
		{
			if (_achievementPack.QuestConfigs.Length == 0)
			{
				questConfig = null;
				return false;
			}

			questConfig = _achievementPack.QuestConfigs[_index];
			_index++;
			_index %= _achievementPack.QuestConfigs.Length;
			return questConfig != null;
		}

		public bool TryLoad(ConfigID<QuestConfig> questId, out QuestConfig questConfig)
			=> _achievementPack.QuestConfigs.TryFindById(questId, out questConfig);
	}
}