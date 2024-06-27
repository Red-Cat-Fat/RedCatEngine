using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Tests.SpecialSubClasses
{
	public class TestQuestFactory : IQuestFactory
	{
		public IQuest ReturnQuest;
		
		public IQuest MakeNewQuest()
		{
			return ReturnQuest;
		}

		public IQuest LoadFrom(IQuestData saveData)
		{
			return ReturnQuest;
		}
	}
}