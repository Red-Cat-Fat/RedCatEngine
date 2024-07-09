using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Exceptions;
using Random = UnityEngine.Random;

namespace RedCatEngine.Quests.Mechanics.QuestGenerators
{
	public class RandomQuestSelector : IQuestSelector
	{
		public string Name
			=> $"RandomQuestSelectorGenerator from {_collection.name}";

		private readonly QuestCollectionConfig _collection;

		public RandomQuestSelector(QuestCollectionConfig collection)
		{
			_collection = collection;
		}

		public QuestConfig GetNextQuest()
		{
			if (_collection.QuestConfigs.Length == 0)
				throw new NotFoundQuestException(this);

			var index = Random.Range(0, _collection.QuestConfigs.Length); //todo: make RandomService
			var questConfig = _collection.QuestConfigs[index];
			return questConfig;
		}

		public bool TryLoad(ConfigID<QuestConfig> questId, out QuestConfig questConfig)
			=> _collection.QuestConfigs.TryFindById(questId, out questConfig);
	}
}