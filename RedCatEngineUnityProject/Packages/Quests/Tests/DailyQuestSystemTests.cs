using NUnit.Framework;
using RedCatEngine.Quests.Mechanics;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Tests.SpecialSubClasses;

namespace RedCatEngine.Quests.Tests
{
	public class DailyQuestSystemTests
	{
		private DailyQuestSystem _dailyQuestSystem;
		private TestTimeService _testTimeService;
		private TestQuestFactory _testFactory;

		[SetUp]
		public void SetUp()
		{
			_testFactory = new TestQuestFactory();
			_testFactory = new TestQuestFactory();
		}
		
		[Test]
		public void GivenDailyQuestSystem_WhenCreateWithEmptyData_ThenCreatedNeedCountQuests()
		{
			var questCount = 3;

			_testFactory.ReturnQuest = new TestQuest();
			
			var system = new DailyQuestSystem(DailyQuestData.Empty,
				_testTimeService,
				_testFactory,
				questCount,
				24 * 60 * 60);
			var activeQuests = system.GetActiveQuest();
			
			Assert.AreEqual(activeQuests.Count, questCount, "Incorrect Count quest");
		}
	}
}