using NUnit.Framework;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Tests.SpecialSubClasses;
using UnityEngine;

namespace RedCatEngine.Quests.Tests
{
	public class BaseCollectProgressQuestTests
	{
		[Test]
		public void GivenDeltaChangeProgressQuest_WhenChangeValuesAndSave_ThenLoadCorrect()
		{
			var questForChange = new TestCollectProgressQuest(ConfigID<QuestConfig>.Invalid, 5);
			Debug.Log($"Create quest. Start progress: {questForChange.GetProcessProgressText()}");
			questForChange.SetCurrentValueForTest(3);
			Debug.Log($"Progress after change: {questForChange.GetProcessProgressText()}");
			var data = questForChange.GetData();
			var questForLoad = new TestCollectProgressQuest(ConfigID<QuestConfig>.Invalid, 5);
			questForLoad.LoadSave(data);
			Debug.Log($"New quest after load: {questForLoad.GetProcessProgressText()}");
			Assert.AreEqual(
				questForChange.GetProcessProgressText(),
				questForLoad.GetProcessProgressText(),
				"Incorrect load data");
		}
	}
}