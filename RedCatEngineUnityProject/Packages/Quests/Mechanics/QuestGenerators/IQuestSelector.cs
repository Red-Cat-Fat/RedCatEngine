using System.Collections.Generic;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Mechanics.QuestGenerators
{
	public interface IQuestSelector
	{
		string Name { get; }
		int TotalQuestVariants { get; }
		bool TryGetNextQuest(List<IQuest> currentActiveQuests, out QuestConfig questConfig);
		bool TryLoad(ConfigID<QuestConfig> questId, out QuestConfig questConfig);
	}
}