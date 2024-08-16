using System.Linq;
using NUnit.Framework;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.QuestSystems;
using RedCatEngine.Quests.Tests.SpecialSubClasses;
using RedCatEngine.Rewards.Base;
using UnityEngine;

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

			_testFactory.SetReturnQuest(
				new IQuest[]
				{
					new TestQuest(),
					new TestQuest(),
					new TestQuest()
				});

			var system = new DailyQuestSystem(
				_testFactory,
				questCount,
				24 * 60 * 60);
			var activeQuests = system.GetActiveQuest();

			Assert.AreEqual(
				activeQuests.Count,
				questCount,
				"Incorrect Count quest");
		}

		[Test]
		public void GivenDailyQuestSystemWithQuests_WhenSkipQuest_ThenActiveQuestIsRestore()
		{
			var skipQuest = new TestQuest();
			_testFactory.SetReturnQuest(
				new IQuest[]
				{
					skipQuest,
					new TestQuest(),
					new TestQuest(),
					new TestQuest(),
					new TestQuest(),
					new TestQuest()
				});

			var system = new DailyQuestSystem(
				_testFactory,
				3,
				24 * 60 * 60);
			skipQuest.Skip();
			var activeQuests = system.GetActiveQuest();

			Assert.AreEqual(
				activeQuests.Sum(quest => quest.QuestState == QuestState.Skip ? 1 : 0),
				0,
				"Skip quest not remove");
			Assert.AreEqual(
				activeQuests.Count,
				3,
				"Incorrect count quest after skip one");
		}

		[Test]
		public void GivenDailyQuestSystemWithQuests_WhenFinishedQuest_ThenActiveQuestLess()
		{
			var skipQuest = new TestQuest();
			_testFactory.SetReturnQuest(
				new IQuest[]
				{
					skipQuest,
					new TestQuest(),
					new TestQuest(),
					new TestQuest(),
					new TestQuest(),
					new TestQuest()
				});

			var system = new DailyQuestSystem(
				_testFactory,
				3,
				24 * 60 * 60);
			skipQuest.Close();
			var activeQuests = system.GetActiveQuest();

			Assert.AreEqual(
				activeQuests.Sum(quest => quest.QuestState == QuestState.Skip ? 1 : 0),
				0,
				"Skip quest not remove");
			Assert.AreEqual(
				activeQuests.Count,
				3,
				"Incorrect count quest after skip one");
		}

		[Test]
		public void GivenDailyQuestSystemWithQuests_WhenSaveData_ThenCorrectLoad()
		{
			var deltaTest = new TestDeltaChangeProgressQuest(
				ConfigID<QuestConfig>.MakeForTest(41),
				IReward.Empty,
				41);
			var deltaTest2 = new TestDeltaChangeProgressQuest(
				ConfigID<QuestConfig>.MakeForTest(42),
				IReward.Empty,
				42);
			var collectTest = new TestCollectProgressQuest(
				ConfigID<QuestConfig>.MakeForTest(43),
				IReward.Empty,
				43);

			Debug.Log("Before load parameters:");
			deltaTest.SetStartValueForTest(21);
			deltaTest.SetCurrentValueForTest(44);
			Debug.LogFormat("41: {0}", deltaTest.ProcessProgressText);
			deltaTest2.SetStartValueForTest(20);
			deltaTest2.SetCurrentValueForTest(44);
			Debug.LogFormat("42: {0}", deltaTest2.ProcessProgressText);
			collectTest.SetCurrentValueForTest(3);
			Debug.LogFormat("43: {0}", collectTest.ProcessProgressText);

			_testFactory.SetReturnQuest(
				new IQuest[]
				{
					deltaTest,
					deltaTest2,
					collectTest
				});

			var systemForSave = new DailyQuestSystem(
				_testFactory,
				3,
				24 * 60 * 60);

			var questContainer = systemForSave.GetData();

			Debug.Log("In get data parameters:");
			var quests = questContainer.GetQuests();
			for (var i = 0; i < quests.Count; i++)
				Debug.LogFormat(
					"[{0}]: {1}",
					i,
					quests[i]);

			var deltaTestAfterLoad = new TestDeltaChangeProgressQuest(
				ConfigID<QuestConfig>.MakeForTest(41),
				IReward.Empty,
				41);
			var deltaTest2AfterLoad = new TestDeltaChangeProgressQuest(
				ConfigID<QuestConfig>.MakeForTest(42),
				IReward.Empty,
				42);
			var collectTestAfterLoad = new TestCollectProgressQuest(
				ConfigID<QuestConfig>.MakeForTest(43),
				IReward.Empty,
				43);

			_testFactory.SetReturnQuest(
				new IQuest[]
				{
					deltaTestAfterLoad,
					deltaTest2AfterLoad,
					collectTestAfterLoad
				});

			var systemForLoad = new DailyQuestSystem(
				_testFactory,
				3,
				24 * 60 * 60);
			systemForLoad.LoadData(questContainer);

			var activeQuestBeforeLoad = systemForSave.GetActiveQuest();
			var activeQuestAfterLoad = systemForLoad.GetActiveQuest();

			Assert.AreEqual(activeQuestBeforeLoad.Count, activeQuestAfterLoad.Count);
			for (var i = 0; i < activeQuestBeforeLoad.Count; i++)
			{
				Debug.Log($"[{i}] Before: {activeQuestBeforeLoad[i].ProcessProgressText}");
				Debug.Log($"[{i}] After: {activeQuestAfterLoad[i].ProcessProgressText}");
				Assert.AreEqual(
					activeQuestBeforeLoad[i].ProcessProgressText,
					activeQuestAfterLoad[i].ProcessProgressText);
			}
		}

		[Test]
		public void GivenDailyQuestSystemWithQuests_WhenSaveQuests_ThenActiveQuestIsRestore()
		{
			var skipQuest = new TestQuest();
			_testFactory.SetReturnQuest(
				new IQuest[]
				{
					skipQuest,
					new TestQuest(),
					new TestQuest(),
					new TestQuest(),
					new TestQuest(),
					new TestQuest()
				});

			var system = new DailyQuestSystem(
				_testFactory,
				3,
				24 * 60 * 60);
			skipQuest.Skip();
			var activeQuests = system.GetActiveQuest();

			Assert.AreEqual(
				activeQuests.Sum(quest => quest.QuestState == QuestState.Skip ? 1 : 0),
				0,
				"Skip quest not remove");
			Assert.AreEqual(
				activeQuests.Count,
				3,
				"Incorrect count quest after skip one");
		}
	}
}