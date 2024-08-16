using System;
using RedCatEngine.Quests.Mechanics.QuestGenerators;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Exceptions
{
	public class CantLoadFromDataException : Exception
	{
		public CantLoadFromDataException(IQuestData questData, IQuestSelector questSelector)
			: base($"Cant load {questData} data from {questSelector.Name} selector") { }
	}
}