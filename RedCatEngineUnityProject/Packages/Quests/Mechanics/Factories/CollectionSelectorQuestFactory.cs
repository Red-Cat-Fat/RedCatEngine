using System.Collections.Generic;
using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Exceptions;
using RedCatEngine.Quests.Mechanics.QuestGenerators;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Factories
{
	public class CollectionSelectorQuestFactory : IQuestFactory
	{
		private readonly IApplicationContainer _applicationContainer;
		private readonly IQuestSelector _randomQuestSelector;

		public CollectionSelectorQuestFactory(IApplicationContainer applicationContainer, IQuestSelector selector)
		{
			_applicationContainer = applicationContainer;
			_randomQuestSelector = selector;
		}

		public IQuest MakeFromConfig(ConfigID<QuestConfig> questConfig)
		{
			_randomQuestSelector.TryLoad(questConfig, out var quest);
			return quest.Make(_applicationContainer);
		}

		public IQuest MakeNewQuest(List<IQuest> currentActiveQuests)
		{
			if (_randomQuestSelector.TryGetNextQuest(currentActiveQuests, out var quest))
				return quest.Make(_applicationContainer);

			throw new NotFoundQuestException(_randomQuestSelector);
		}

		public IQuest LoadFrom(IQuestData saveData)
		{
			return _randomQuestSelector.TryLoad(saveData.GetConfig(), out var quest)
				? quest.Make(_applicationContainer, saveData)
				: throw new CantLoadFromDataException(saveData, _randomQuestSelector);
		}

		public bool TryLoad(ConfigID<QuestConfig> questId, out QuestConfig questConfig)
			=> _randomQuestSelector.TryLoad(questId, out questConfig);
	}
}