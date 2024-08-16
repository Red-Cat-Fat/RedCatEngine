using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;
using RedCatEngine.Rewards.Base;

namespace RedCatEngine.Quests.Tests.SpecialSubClasses
{
	public class TestQuest : IQuest
	{
		public event Action<IQuest> ChangeQuestStateEvent;
		public event Action<IQuest> ChangeProgressEvent;
		public ConfigID<QuestConfig> Config { get; set; }
		public IReward Reward { get; }
		public QuestState QuestState { get; set; }
		public double Progress { get; set; }
		public string ProcessProgressText { get; set; }

		public void Start(DateTime time)
		{
			QuestState = QuestState.InProgress;
			ChangeQuestStateEvent?.Invoke(this);
		}

		public void Continue()
		{
			throw new NotImplementedException();
		}

		public void Close()
		{
			QuestState = QuestState.Complete;
			ChangeQuestStateEvent?.Invoke(this);
		}

		public void Skip()
		{
			QuestState = QuestState.Skip;
			ChangeQuestStateEvent?.Invoke(this);
		}

		public void Finished()
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

		public string GetDescription()
		{
			throw new NotImplementedException();
		}
	}
}