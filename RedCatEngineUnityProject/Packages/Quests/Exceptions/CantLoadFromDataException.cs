using System;
using RedCatEngine.Quests.Mechanics.QuestGenerators;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Exceptions
{
	public class CantLoadFromDataException : Exception
	{
		public CantLoadFromDataException(IQuestData questData, IQuestSelector questSelector)
			: base(string.Format("Cant load {0} data from {1} selector", questData, questSelector.Name)) { }
	}
}