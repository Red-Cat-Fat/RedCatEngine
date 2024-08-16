using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;

namespace RedCatEngine.Quests.Configs.QuestCollections
{
	public interface IQuestCollection
	{
		int Count { get; }
		QuestConfig this[int index] { get; }
		bool TryFindById(ConfigID<QuestConfig> questId, out QuestConfig questConfig);
		string GetName();
	}
}