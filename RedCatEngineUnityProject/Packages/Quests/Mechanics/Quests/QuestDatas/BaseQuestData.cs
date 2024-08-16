using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Utils;
using UnityEngine;

namespace RedCatEngine.Quests.Mechanics.Quests.QuestDatas
{
	[Serializable]
	public class BaseQuestData : IQuestData
	{
		public ConfigID<QuestConfig> GetConfig()
			=> _config;

		public DateTime GetCreateTime()
			=> (DateTime)_createTime;

		public QuestState GetQuestState()
			=> _questState;

		[SerializeField]
		private ConfigID<QuestConfig> _config;
		[SerializeField]
		private SerializedDateTime _createTime;
		[SerializeField]
		private QuestState _questState;

		public void SetQuestState(QuestState newQuestState)
			=> _questState = newQuestState;

		public void ConstructSerialization(
			ConfigID<QuestConfig> config,
			DateTime createTime,
			QuestState questState
		)
		{
			_config = config;
			_createTime = (SerializedDateTime)createTime;
			_questState = questState;
		}

		public override string ToString()
		{
			return $"Config: {_config}; DateTime: {_createTime}; QuestState: {_questState};";
		}
	}
}