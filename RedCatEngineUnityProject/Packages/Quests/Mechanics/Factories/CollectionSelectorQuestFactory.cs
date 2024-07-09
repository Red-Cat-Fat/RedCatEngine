using RedCatEngine.DependencyInjection.Containers;
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

		public IQuest MakeNewQuest()
		{
			var quest = _randomQuestSelector.GetNextQuest();
			return quest.Make(_applicationContainer);
		}

		public IQuest LoadFrom(IQuestData saveData)
		{
			return _randomQuestSelector.TryLoad(saveData.Config, out var quest)
				? quest.Make(_applicationContainer) 
				: throw new CantLoadFromDataException(saveData, _randomQuestSelector);
		}
	}
}