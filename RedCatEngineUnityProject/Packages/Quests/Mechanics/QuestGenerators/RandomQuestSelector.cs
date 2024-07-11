using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs;
using RedCatEngine.Quests.Configs.Quests;
using Random = UnityEngine.Random;

namespace RedCatEngine.Quests.Mechanics.QuestGenerators
{
	public class RandomQuestSelector : IQuestSelector
	{
		public string Name
			=> $"RandomQuestSelectorGenerator from {_collection.name}";

		public int TotalQuestVariants
			=> _collection.QuestConfigs.Length;

		private readonly QuestCollectionConfig _collection;

		public RandomQuestSelector(QuestCollectionConfig collection)
		{
			_collection = collection;
		}

		public bool TryGetNextQuest(out QuestConfig questConfig)
		{
			if (_collection.QuestConfigs.Length == 0)
			{
				questConfig = null;
				return false;
			}

			var index = Random.Range(0, _collection.QuestConfigs.Length); //todo: make RandomService
			questConfig = _collection.QuestConfigs[index];
			return questConfig != null;
		}

		public bool TryLoad(ConfigID<QuestConfig> questId, out QuestConfig questConfig)
			=> _collection.QuestConfigs.TryFindById(questId, out questConfig);
	}
}