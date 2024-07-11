using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Tests.SpecialSubClasses
{
	public class TestQuestData : IQuestData
	{
		public ConfigID<QuestConfig> Config => ConfigID<QuestConfig>.Invalid;
		public DateTime CreateTime { get; }
		public QuestState QuestState { get; }
	}
}