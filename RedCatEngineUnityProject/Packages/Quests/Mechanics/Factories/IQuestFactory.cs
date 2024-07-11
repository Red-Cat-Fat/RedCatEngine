using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Factories
{
	public interface IQuestFactory
	{
		IQuest MakeFromConfig(ConfigID<QuestConfig> questConfig);
		IQuest MakeNewQuest();
		IQuest LoadFrom(IQuestData saveData);
	}
}