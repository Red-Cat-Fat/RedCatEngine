using System;
using NUnit.Framework;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;
using RedCatEngine.Quests.Tests.SpecialSubClasses;

namespace RedCatEngine.Quests.Tests
{
	public class QuestsDataContainerTests
	{
		[Test]
		public void GivenCustomQuestData_WhenAddQuest_ThenStoredAndReturned()
		{
			var container = new QuestsDataContainer();
			var data = new TestQuestData();
			data.SetQuestState(QuestState.Complete);

			Assert.DoesNotThrow(() => container.AddQuest(data), "Custom quest data must be supported");
			CollectionAssert.Contains(container.GetQuests(), data);
		}

		[Test]
		public void GivenBuiltInAndCustomQuestData_WhenGetQuests_ThenAllReturned()
		{
			var container = new QuestsDataContainer();
			container.AddQuest(new CollectProgressQuestData());
			container.AddQuest(new DeltaChangeProgressQuestData());
			container.AddQuest(new TestQuestData());

			Assert.AreEqual(3, container.GetQuests().Count);
		}

		[Test]
		public void GivenNullQuestData_WhenAddQuest_ThenThrowsArgumentNull()
		{
			var container = new QuestsDataContainer();

			Assert.Throws<ArgumentNullException>(() => container.AddQuest(null));
		}

		[Test]
		public void GivenNewContainerWithQuest_WhenCompareWithEmpty_ThenNotEqual()
		{
			var container = new QuestsDataContainer();
			container.AddQuest(new TestQuestData());

			Assert.IsTrue(container != QuestsDataContainer.Empty);
		}
	}
}
