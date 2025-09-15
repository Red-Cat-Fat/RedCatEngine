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
		private string _processProgressText;
		private double _progress;
		public IReward Reward { get; }
		public double Progress
		{
			set => _progress = value;
		}

		public string ProcessProgressText
		{
			set => _processProgressText = value;
		}
		public event Action<IQuest> ChangeQuestStateEvent;
		public event Action<IQuest> ChangeProgressEvent;
		public ConfigID<QuestConfig> Config { get; set; }
		public QuestState QuestState { get; set; }

		public double GetProgress()
			=> _progress;

		public string GetProcessProgressText()
			=> _processProgressText;

		public void Start(DateTime time)
		{
			QuestState = QuestState.InProgress;
			ChangeQuestStateEvent?.Invoke(this);
		}

		public void Continue()
		{
			throw new NotImplementedException();
		}

		public void Disable()
		{
			QuestState = QuestState.Complete;
			ChangeQuestStateEvent?.Invoke(this);
		}

		public void Skip()
		{
			QuestState = QuestState.Skip;
			ChangeQuestStateEvent?.Invoke(this);
		}

		public void SuccessFinished()
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

		public string GetLocalizedName()
		{
			throw new NotImplementedException();
		}

		public string GetLocalizedDescription()
		{
			throw new NotImplementedException();
		}
	}
}