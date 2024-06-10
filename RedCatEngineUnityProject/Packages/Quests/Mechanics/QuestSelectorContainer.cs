using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs;
using RedCatEngine.Quests.Configs.Quests;
using Random = UnityEngine.Random;

namespace RedCatEngine.Quests.Mechanics
{
	public class QuestSelectorContainer
	{
		private readonly QuestCollectionConfig _collection;

		public QuestSelectorContainer(QuestCollectionConfig collection)
		{
			_collection = collection;
		}

		public QuestConfig GetRandomQuest()
		{
			if (_collection.QuestConfigs.Length == 0)
				return null; //todo: make Exceptions

			var index = Random.Range(0, _collection.QuestConfigs.Length); //todo: make RandomService
			var questConfig = _collection.QuestConfigs[index];
			return questConfig;
		}

		public bool TryLoad(ConfigID<QuestConfig> id, out QuestConfig questConfig)
			=> _collection.QuestConfigs.TryFindById(id, out questConfig);
	}
}