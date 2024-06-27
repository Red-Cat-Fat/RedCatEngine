using System;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Tests.SpecialSubClasses
{
	public class TestQuest : IQuest
	{
		public event Action CompleteEvent;
		public QuestState QuestState { get; }
		public float Progress { get; }
		public string ProcessProgressText { get; }
		public void Start(DateTime time)
		{
			
		}

		public void Close()
		{
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