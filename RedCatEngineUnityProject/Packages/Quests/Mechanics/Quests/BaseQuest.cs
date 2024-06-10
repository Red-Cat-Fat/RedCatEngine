using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public abstract class BaseQuest : IQuest
	{
		public event Action CompleteEvent;
		public abstract QuestState QuestState { get; }
		public abstract float Progress { get; }
		public abstract string ProcessProgressText { get; }

		protected readonly ConfigID<QuestConfig> Config;

		protected BaseQuest(ConfigID<QuestConfig> config)
		{
			Config = config;
		}

		public void Start(DateTime time)
		{
			DoStart(time);
		}

		public void Close()
		{
			DoClose();
		}

		protected virtual void SendComplete() 
			=> CompleteEvent?.Invoke();

		protected abstract void DoStart(DateTime dateTime);
		protected abstract void DoClose();

		public abstract IQuest LoadSave(IQuestData data);
		public abstract IQuestData GetData();
	}
}