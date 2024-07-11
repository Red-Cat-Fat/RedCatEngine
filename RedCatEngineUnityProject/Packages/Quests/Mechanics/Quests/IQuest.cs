using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public interface IQuest
	{
		event Action<ConfigID<QuestConfig>> ChangeQuestStateEvent;
		ConfigID<QuestConfig> Config { get; }
		QuestState QuestState { get; }
		float Progress { get; }
		string ProcessProgressText { get; }
		void Start(DateTime time);
		void Close();
		void Skip();
		IQuest LoadSave(IQuestData data);
		IQuestData GetData();
	}
}