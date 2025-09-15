using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public abstract class BaseQuest : IQuest
	{
		protected BaseQuest(ConfigID<QuestConfig> config)
		{
			Config = config;
		}

		protected DateTime StartQuestTime { get; private set; }
		public event Action<IQuest> ChangeQuestStateEvent;
		public event Action<IQuest> ChangeProgressEvent;

		public ConfigID<QuestConfig> Config { get; }
		public QuestState QuestState { get; private set; }

		public abstract double GetProgress();
		public abstract string GetProcessProgressText();

		public abstract string GetLocalizedName();
		public abstract string GetLocalizedDescription();

		public abstract IQuest LoadSave(IQuestData data);
		public abstract IQuestData GetData();

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

		public void Disable()
			=> DoReset();

		public void Skip()
		{
			if (QuestState is not QuestState.Finished)
				SetQuestState(QuestState.Skip);
			Disable();
		}

		public void SuccessFinished()
		{
			if (QuestState is QuestState.Complete or QuestState.InProgress)
				SetQuestState(QuestState.Finished);
			DoSuccessFinished();
			Disable();
		}

		protected abstract void CheckComplete();
		protected abstract void DoResetValue();
		protected abstract void DoStart();
		protected abstract void DoReset();
		protected abstract void DoSuccessFinished();

		protected void SetStartTime(DateTime time)
			=> StartQuestTime = time;

		protected void SetQuestState(QuestState newState)
		{
			if (newState == QuestState)
				return;
			if (newState != QuestState.InProgress)
				Disable();

			QuestState = newState;
			ChangeQuestStateEvent?.Invoke(this);
		}

		protected void SendComplete()
		{
			if (QuestState == QuestState.InProgress)
				SetQuestState(QuestState.Complete);
		}

		protected void UpdateProgress()
			=> ChangeProgressEvent?.Invoke(this);
	}
}