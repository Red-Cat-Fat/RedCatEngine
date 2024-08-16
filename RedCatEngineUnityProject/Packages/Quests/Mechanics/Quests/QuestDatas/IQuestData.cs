using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;

namespace RedCatEngine.Quests.Mechanics.Quests.QuestDatas
{
	public interface IQuestData
	{
		ConfigID<QuestConfig> GetConfig();
		DateTime GetCreateTime();
		QuestState GetQuestState();
		
		void SetQuestState(QuestState newQuestState);
		void ConstructSerialization(ConfigID<QuestConfig> config, DateTime createTime, QuestState questState);
	}
}