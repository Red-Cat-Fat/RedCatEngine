using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public abstract class BaseQuest : IQuest
	{
		public event Action<ConfigID<QuestConfig>> ChangeQuestStateEvent;

		public ConfigID<QuestConfig> Config { get; }
		public QuestState QuestState { get; private set; }
		public abstract float Progress { get; }
		public abstract string ProcessProgressText { get; }

		protected BaseQuest(ConfigID<QuestConfig> config)
		{
			Config = config;
		}

		private void SetState(QuestState newState)
		{
			if(newState == QuestState)
				return;
			QuestState = newState;
			ChangeQuestStateEvent?.Invoke(Config);
		}
		
		public void Start(DateTime time)
			=> DoStart(time);

		public void Close()
			=> DoClose();

		public void Skip()
			=> SetState(QuestState.Skip);

		protected virtual void SendComplete()
			=> SetState(QuestState.Complete);

		protected abstract void DoStart(DateTime dateTime);
		protected abstract void DoClose();

		public abstract IQuest LoadSave(IQuestData data);
		public abstract IQuestData GetData();
	}
}