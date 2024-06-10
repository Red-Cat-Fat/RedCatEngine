using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Factories
{
	public class QuestCollectionSelectorQuestFactory : IQuestFactory
	{
		private readonly IApplicationContainer _applicationContainer;
		private readonly QuestSelectorContainer _questSelector;

		public QuestCollectionSelectorQuestFactory(IApplicationContainer applicationContainer, QuestSelectorContainer container)
		{
			_applicationContainer = applicationContainer;
			_questSelector = container;
		}

		public IQuest MakeNewQuest()
		{
			var quest = _questSelector.GetRandomQuest();
			return quest.Make(_applicationContainer);
		}

		public IQuest LoadFrom(IQuestData saveData)
		{
			return _questSelector.TryLoad(saveData.Config, out var quest)
				? quest.Make(_applicationContainer) 
				: null; //todo: make Exceptions
		}
	}
}