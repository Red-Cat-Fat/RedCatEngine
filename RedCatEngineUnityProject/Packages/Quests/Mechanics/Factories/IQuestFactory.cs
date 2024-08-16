using System.Collections.Generic;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Factories
{
	public interface IQuestFactory
	{
		IQuest MakeFromConfig(ConfigID<QuestConfig> questConfig);
		IQuest MakeNewQuest(List<IQuest> currentActiveQuests);
		IQuest LoadFrom(IQuestData saveData);
		bool TryLoad(ConfigID<QuestConfig> questId, out QuestConfig questConfig);
	}
}