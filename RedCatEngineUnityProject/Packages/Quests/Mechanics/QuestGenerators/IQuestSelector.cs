using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;

namespace RedCatEngine.Quests.Mechanics.QuestGenerators
{
	public interface IQuestSelector
	{
		string Name { get; }
		int TotalQuestVariants { get; }
		bool TryGetNextQuest(out QuestConfig questConfig);
		bool TryLoad(ConfigID<QuestConfig> questId, out QuestConfig questConfig);
	}
}