using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.QuestGenerators;

namespace RedCatEngine.Quests.Exceptions
{
	public class NotFoundQuestException : Exception
	{
		public NotFoundQuestException()
			: base($"Not found quest.")
		{
		}

		public NotFoundQuestException(ConfigID<QuestConfig> questConfig)
			: base($"In collection selector not found quest (id:{questConfig})")
		{
		}

		public NotFoundQuestException(IQuestSelector randomQuestSelectorSelector)
			: base($"In {randomQuestSelectorSelector.Name} selector not found quest")
		{
		}
	}
}