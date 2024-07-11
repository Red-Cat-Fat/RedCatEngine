using NUnit.Framework;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.QuestSystems;
using RedCatEngine.Quests.Tests.SpecialSubClasses;

namespace RedCatEngine.Quests.Tests
{
	public class DailyQuestSystemTests
	{
		private DailyQuestSystem _dailyQuestSystem;
		private TestQuestFactory _testFactory;

		[SetUp]
		public void SetUp()
		{
			_testFactory = new TestQuestFactory();
		}

		[Test]
		public void GivenDailyQuestSystem_WhenCreateWithEmptyData_ThenCreatedNeedCountQuests()
		{
			var questCount = 3;

			_testFactory.ReturnQuest = new TestQuest();

			var system = new DailyQuestSystem(DailyQuestsData.Empty,
				_testFactory,
				questCount,
				24 * 60 * 60);
			var activeQuests = system.GetActiveQuest();

			Assert.AreEqual(activeQuests.Count,
				questCount,
				"Incorrect Count quest");
		}
		[Test]
		public void GivenDailyQuestSystemWithQuests_WhenSkipQuest_ThenActiveQuestLess()
		{
			var questCount = 3;

			_testFactory.ReturnQuest = new TestQuest();

			var system = new DailyQuestSystem(DailyQuestsData.Empty,
				_testFactory,
				questCount,
				24 * 60 * 60);
			var activeQuests = system.GetActiveQuest();

			Assert.AreEqual(activeQuests.Count,
				42,
				"Incorrect Count quest");
		}
	}
}