using System.Collections.Generic;
using System.Linq;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.QuestCollections;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using Random = UnityEngine.Random;

namespace RedCatEngine.Quests.Mechanics.QuestGenerators
{
	public class RandomQuestSelector : IQuestSelector
	{
		public string Name
			=> $"RandomQuestSelectorGenerator from {_collection.GetName()}";

		public int TotalQuestVariants
			=> _collection.Count;

		private readonly IQuestCollection _collection;

		public RandomQuestSelector(SimpleQuestCollectionConfig collection)
			=> _collection = collection;

		public bool TryGetNextQuest(List<IQuest> currentActiveQuests, out QuestConfig questConfig)
		{
			if (_collection.Count == 0)
			{
				questConfig = null;
				return false;
			}

			var tryCount = 10;
			do
			{
				var index = Random.Range(0, _collection.Count); //todo: make RandomService
				while (currentActiveQuests.Count(quest => quest.Config == _collection[index]) > 0 &&
				       tryCount > 0)
				{
					index++;
					index %= currentActiveQuests.Count;
					tryCount--;
				}

				questConfig = _collection[index];
				tryCount--;
			} while (questConfig == null && tryCount > 0);

			return questConfig != null;
		}

		public bool TryLoad(ConfigID<QuestConfig> questId, out QuestConfig questConfig)
			=> _collection.TryFindById(questId, out questConfig);
	}
}