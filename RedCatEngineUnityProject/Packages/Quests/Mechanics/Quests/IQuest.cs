using System;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public interface IQuest : IQuestDescription
	{
		event Action<IQuest> ChangeQuestStateEvent;
		event Action<IQuest> ChangeProgressEvent;

		void Start(DateTime time);
		void Continue();
		void Disable();
		void Skip();
		void SuccessFinished();
		IQuest LoadSave(IQuestData data);
		IQuestData GetData();
	}
}