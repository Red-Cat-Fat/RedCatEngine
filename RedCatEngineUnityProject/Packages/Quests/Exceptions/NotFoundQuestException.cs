using System;
using RedCatEngine.Quests.Mechanics.QuestGenerators;

namespace RedCatEngine.Quests.Exceptions
{
	public class NotFoundQuestException : Exception
	{
		public NotFoundQuestException(IQuestSelector randomQuestSelectorSelector)
			: base($"In {randomQuestSelectorSelector.Name} selector not found quest")
		{
		}
	}
}