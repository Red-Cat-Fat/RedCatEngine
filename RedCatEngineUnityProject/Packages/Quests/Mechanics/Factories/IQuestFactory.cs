using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Factories
{
	public interface IQuestFactory
	{
		IQuest MakeNewQuest();
		IQuest LoadFrom(IQuestData saveData);
	}
}