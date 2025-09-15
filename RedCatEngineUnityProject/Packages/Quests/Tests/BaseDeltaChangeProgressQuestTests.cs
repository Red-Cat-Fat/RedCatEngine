using NUnit.Framework;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Tests.SpecialSubClasses;
using UnityEngine;

namespace RedCatEngine.Quests.Tests
{
	public class BaseDeltaChangeProgressQuestTests
	{
		[Test]
		public void GivenDeltaChangeProgressQuest_WhenChangeValuesAndSave_ThenLoadCorrect()
		{
			var questForChange = new TestDeltaChangeProgressQuest(ConfigID<QuestConfig>.Invalid, 5);
			Debug.Log($"Create quest. Start progress: {questForChange.GetProcessProgressText()}");
			questForChange.SetStartValueForTest(1);
			questForChange.SetCurrentValueForTest(3);
			Debug.Log($"Progress after change: {questForChange.GetProcessProgressText()}");
			var data = questForChange.GetData();
			var questForLoad = new TestDeltaChangeProgressQuest(ConfigID<QuestConfig>.Invalid, 5);
			questForLoad.LoadSave(data);
			Debug.Log($"New quest after load: {questForLoad.GetProcessProgressText()}");

			Assert.AreEqual(
				questForChange.GetProcessProgressText(),
				questForLoad.GetProcessProgressText(),
				"Incorrect load data");
		}
	}
}