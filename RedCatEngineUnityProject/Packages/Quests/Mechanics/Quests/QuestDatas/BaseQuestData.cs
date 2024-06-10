using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;

namespace RedCatEngine.Quests.Mechanics.Quests.QuestDatas
{
	public class BaseQuestData : IQuestData
	{
		public ConfigID<QuestConfig> Config { get; set; }
		public DateTime CreateTime { get; set; }
		public QuestState QuestState { get; set; }
	}
}