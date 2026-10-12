using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Tests.SpecialSubClasses
{
	[Serializable]
	public class TestQuestData : IQuestData
	{
		public ConfigID<QuestConfig> Config { get; private set; } = ConfigID<QuestConfig>.Invalid;
		public DateTime CreateTime { get; private set; }
		public QuestState QuestState { get; private set; }

		public ConfigID<QuestConfig> GetConfig()
			=> Config;

		public DateTime GetCreateTime()
			=> CreateTime;

		public QuestState GetQuestState()
			=> QuestState;

		public void SetQuestState(QuestState newQuestState)
			=> QuestState = newQuestState;

		public void ConstructSerialization(
			ConfigID<QuestConfig> config,
			DateTime createTime,
			QuestState questState
		)
		{
			Config = config;
			CreateTime = createTime;
			QuestState = questState;
		}
	}
}