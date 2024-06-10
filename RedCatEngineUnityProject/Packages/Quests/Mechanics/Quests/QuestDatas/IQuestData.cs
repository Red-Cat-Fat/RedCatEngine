using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;

namespace RedCatEngine.Quests.Mechanics.Quests.QuestDatas
{
	public interface IQuestData
	{
		public ConfigID<QuestConfig> Config { get; }
		public DateTime CreateTime { get; }
		public QuestState QuestState { get; }
	}
}