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
		private ConfigID<QuestConfig> _config = ConfigID<QuestConfig>.Invalid;
		private DateTime _createTime = DateTime.UtcNow;
		private QuestState _questState = QuestState.InProgress;

		public ConfigID<QuestConfig> Config
			=> _config;

		public DateTime CreateTime
			=> _createTime;

		public QuestState QuestState
			=> _questState;

		public ConfigID<QuestConfig> GetConfig()
		{
			return Config;
		}

		public DateTime GetCreateTime()
		{
			return CreateTime;
		}

		public QuestState GetQuestState()
		{
			return QuestState;
		}

		public void SetQuestState(QuestState newQuestState)
		{
			_questState = newQuestState;
		}

		public void ConstructSerialization(
			ConfigID<QuestConfig> config,
			DateTime createTime,
			QuestState questState
		)
		{
			_config = config;
			_createTime = createTime;
			_questState = questState;
		}
	}
}
