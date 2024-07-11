using System;
using RedCatEngine.Quests.Mechanics.QuestGenerators;

namespace RedCatEngine.Quests.Exceptions
{
	public class NotFoundQuestException : Exception
	{
		public NotFoundQuestException(IQuestSelector randomQuestSelectorSelector)
			: base(string.Format("In {0} selector not found quest", randomQuestSelectorSelector.Name))
		{
		}
	}
}