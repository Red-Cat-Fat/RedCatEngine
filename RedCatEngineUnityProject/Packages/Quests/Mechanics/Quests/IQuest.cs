using System;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public interface IQuest
	{
		event Action CompleteEvent;
		QuestState QuestState { get; }
		float Progress { get; }
		string ProcessProgressText { get; }
		void Start(DateTime time);
		void Close();
		IQuest LoadSave(IQuestData data);
		IQuestData GetData();
	}
}