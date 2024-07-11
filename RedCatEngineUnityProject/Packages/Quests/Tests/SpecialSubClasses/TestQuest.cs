using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Tests.SpecialSubClasses
{
	public class TestQuest : IQuest
	{
		public event Action<ConfigID<QuestConfig>> ChangeQuestStateEvent;
		public ConfigID<QuestConfig> Config { get; set; }
		public QuestState QuestState { get; set; }
		public float Progress { get; set; }
		public string ProcessProgressText { get; set; }

		public void Start(DateTime time)
		{
			QuestState = QuestState.InProgress;
			ChangeQuestStateEvent?.Invoke(Config);
		}

		public void Close()
		{
			QuestState = QuestState.Complete;
			ChangeQuestStateEvent?.Invoke(Config);
		}

		public void Skip()
		{
			QuestState = QuestState.Skip;
			ChangeQuestStateEvent?.Invoke(Config);
		}

		public IQuest LoadSave(IQuestData data)
		{
			return this;
		}

		public IQuestData GetData()
		{
			return new TestQuestData();
		}
	}
}