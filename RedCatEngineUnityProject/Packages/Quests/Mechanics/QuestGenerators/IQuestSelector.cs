using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;

namespace RedCatEngine.Quests.Mechanics.QuestGenerators
{
	public interface IQuestSelector
	{
		string Name { get; }
		QuestConfig GetNextQuest();
		bool TryLoad(ConfigID<QuestConfig> questId, out QuestConfig questConfig);
	}
}