using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;
using RedCatEngine.Rewards.Base;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public interface IQuest
	{
		event Action<IQuest> ChangeQuestStateEvent;
		event Action<IQuest> ChangeProgressEvent;
		ConfigID<QuestConfig> Config { get; }
		public IReward Reward { get; }
		QuestState QuestState { get; }
		double Progress { get; }
		string ProcessProgressText { get; }
		void Start(DateTime time);
		void Continue();
		void Close();
		void Skip();
		void Finished();
		IQuest LoadSave(IQuestData data);
		IQuestData GetData();
		public string GetDescription();
	}
}