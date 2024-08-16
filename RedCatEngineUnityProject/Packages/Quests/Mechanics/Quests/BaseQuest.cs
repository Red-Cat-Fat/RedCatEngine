using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;
using RedCatEngine.Rewards.Base;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public abstract class BaseQuest : IQuest
	{
		public event Action<IQuest> ChangeQuestStateEvent;
		public event Action<IQuest> ChangeProgressEvent;

		public ConfigID<QuestConfig> Config { get; }
		public IReward Reward { get; }
		protected DateTime StartQuestTime { get; private set; }
		public QuestState QuestState { get; private set; }
		public abstract double Progress { get; }
		public abstract string ProcessProgressText { get; }

		protected BaseQuest(ConfigID<QuestConfig> config, IReward reward)
		{
			Config = config;
			Reward = reward;
		}

		protected void SetStartTime(DateTime time)
			=> StartQuestTime = time;

		protected void SetQuestState(QuestState newState)
		{
			if (newState == QuestState)
				return;

			QuestState = newState;
			ChangeQuestStateEvent?.Invoke(this);
		}

		public void Start(DateTime time)
		{
			SetStartTime(time);
			DoResetValue();
			DoStart();
		}

		public void Continue()
		{
			CheckComplete();
			DoStart();
		}

		public void Close()
			=> DoClose();

		public void Skip()
		{
			if (QuestState is not QuestState.Finished)
				SetQuestState(QuestState.Skip);
			Close();
		}

		public void Finished()
		{
			if (QuestState is QuestState.Complete or QuestState.InProgress)
				SetQuestState(QuestState.Finished);
		}

		protected void SendComplete()
		{
			if (QuestState == QuestState.InProgress)
				SetQuestState(QuestState.Complete);
			Close();
		}

		protected abstract void CheckComplete();
		protected abstract void DoResetValue();
		protected abstract void DoStart();
		protected abstract void DoClose();

		public abstract IQuest LoadSave(IQuestData data);
		public abstract IQuestData GetData();
		public abstract string GetDescription();

		protected void UpdateProgress()
			=> ChangeProgressEvent?.Invoke(this);
	}
}