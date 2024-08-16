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
		public ConfigID<QuestConfig> Config => ConfigID<QuestConfig>.Invalid;
		public DateTime CreateTime { get; }
		public QuestState QuestState { get; }
		public ConfigID<QuestConfig> GetConfig()
		{
			return Config;
		}

		public DateTime GetCreateTime()
		{
			throw new NotImplementedException();
		}

		public QuestState GetQuestState()
		{
			throw new NotImplementedException();
		}

		public void SetQuestState(QuestState newQuestState)
		{
			throw new NotImplementedException();
		}

		public void ConstructSerialization(
			ConfigID<QuestConfig> config,
			DateTime createTime,
			QuestState questState
		)
		{
			throw new NotImplementedException();
		}
	}
}