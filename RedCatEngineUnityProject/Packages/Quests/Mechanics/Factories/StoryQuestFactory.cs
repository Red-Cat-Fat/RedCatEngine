using System.Collections.Generic;
using System.Linq;
using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Exceptions;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Factories
{
	public class StoryQuestFactory : IQuestFactory
	{
		private readonly List<QuestConfig> _allQuests;
		private readonly IApplicationContainer _applicationContainer;

		[Inject]
		public StoryQuestFactory(IApplicationContainer applicationContainer, AllQuestLinksConfig allQuests)
		{
			_applicationContainer = applicationContainer;
			_allQuests = allQuests.Quests;
		}

		public IQuest MakeFromConfig(ConfigID<QuestConfig> questId)
		{
			if (!TryLoad(questId, out var quest))
				throw new NotFoundQuestException(questId);
			return quest.Make(_applicationContainer);
		}

		public IQuest MakeNewQuest(List<IQuest> currentActiveQuests)
		{
			foreach (var storyLineQuest in _allQuests)
			{
				if (currentActiveQuests.All(quest => quest.Config != storyLineQuest))
				{
					return storyLineQuest.Make(_applicationContainer);
				}
			}
			throw new NotFoundQuestException();
		}

		public IQuest LoadFrom(IQuestData saveData)
		{
			if (!TryLoad(saveData.GetConfig(), out var quest))
				throw new CantLoadFromDataException(saveData);
			return quest.Make(_applicationContainer, saveData);
		}

		public bool TryLoad(ConfigID<QuestConfig> questId, out QuestConfig questConfig)
		{
			foreach (var quest in _allQuests)
			{
				if (quest == questId)
				{
					questConfig = quest;
					return true;
				}
			}
			questConfig = null;
			return false;
		}
	}
}