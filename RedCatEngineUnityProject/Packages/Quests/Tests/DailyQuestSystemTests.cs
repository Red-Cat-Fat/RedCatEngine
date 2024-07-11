using System.Linq;
using NUnit.Framework;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Quests;
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

			_testFactory.SetReturnQuest(new IQuest[]
			{
				new TestQuest(),
				new TestQuest(),
				new TestQuest()
			});

			var system = new DailyQuestSystem(QuestsDataContainer.Empty,
				_testFactory,
				questCount,
				24 * 60 * 60);
			var activeQuests = system.GetActiveQuest();

			Assert.AreEqual(activeQuests.Count,
				questCount,
				"Incorrect Count quest");
		}

		[Test]
		public void GivenDailyQuestSystemWithQuests_WhenSkipQuest_ThenActiveQuestIsRestore()
		{
			var skipQuest = new TestQuest();
			_testFactory.SetReturnQuest(new IQuest[]
			{
				skipQuest,
				new TestQuest(),
				new TestQuest(),
				new TestQuest(),
				new TestQuest(),
				new TestQuest()
			});

			var system = new DailyQuestSystem(QuestsDataContainer.Empty,
				_testFactory,
				3,
				24 * 60 * 60);
			skipQuest.Skip();
			var activeQuests = system.GetActiveQuest();

			Assert.AreEqual(activeQuests.Sum(quest=>quest.QuestState == QuestState.Skip ? 1 : 0), 0, "Skip quest not remove");
			Assert.AreEqual(activeQuests.Count, 3, "Incorrect count quest after skip one");
		}

		[Test]
		public void GivenDailyQuestSystemWithQuests_WhenFinishedQuest_ThenActiveQuestLess()
		{
			var skipQuest = new TestQuest();
			_testFactory.SetReturnQuest(new IQuest[]
			{
				skipQuest,
				new TestQuest(),
				new TestQuest(),
				new TestQuest(),
				new TestQuest(),
				new TestQuest()
			});

			var system = new DailyQuestSystem(QuestsDataContainer.Empty,
				_testFactory,
				3,
				24 * 60 * 60);
			skipQuest.Close();
			var activeQuests = system.GetActiveQuest();

			Assert.AreEqual(activeQuests.Sum(quest=>quest.QuestState == QuestState.Skip ? 1 : 0), 0, "Skip quest not remove");
			Assert.AreEqual(activeQuests.Count, 3, "Incorrect count quest after skip one");
		}
	}
}