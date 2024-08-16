using System.Collections.Generic;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.QuestCollections;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Mechanics.QuestGenerators
{
	public class QueueQuestSelector : IQuestSelector
	{
		private readonly IQuestCollection _questPack;

		public string Name
			=> $"QueueQuestSelector from {_questPack.GetName()}";

		public int TotalQuestVariants
			=> _questPack.Count;

		private int _index = 0;

		public QueueQuestSelector(IQuestCollection questPack)
			=> _questPack = questPack;

		public bool TryGetNextQuest(List<IQuest> currentActiveQuests, out QuestConfig questConfig)
		{
			if (_questPack.Count == 0)
			{
				questConfig = null;
				return false;
			}

			questConfig = _questPack[_index];
			_index++;
			_index %= _questPack.Count;
			return questConfig != null;
		}

		public bool TryLoad(ConfigID<QuestConfig> questId, out QuestConfig questConfig)
			=> _questPack.TryFindById(questId, out questConfig);
	}
}