using System;
using System.Collections.Generic;
using System.Linq;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Mechanics.QuestSystems
{
	public abstract class BaseQuestSystem
	{
		public event Action<IQuest> NewQuestEvent;
		protected readonly List<IQuest> _activeQuest = new();

		protected static DateTime CurrentTime
			=> DateTime.UtcNow; //todo: make time service

		protected readonly IQuestFactory _questFactory;

		protected BaseQuestSystem(IQuestFactory questFactory)
		{
			_questFactory = questFactory;
		}

		protected void LoadData(QuestsDataContainer dailyQuestsData)
		{
			_activeQuest.AddRange(
				dailyQuestsData.ActiveQuests
					.Select(questData => _questFactory.LoadFrom(questData))
					.Where(quest => quest != null));
		}

		public QuestsDataContainer GetData()
		{
			var data = new QuestsDataContainer();
			foreach (var quest in _activeQuest)
				data.ActiveQuests.Add(quest.GetData());

			return data;
		}

		public List<IQuest> GetActiveQuest()
		{
			return _activeQuest;
		}

		protected IQuest CreateAndStartNewQuest()
		{
			var newQuest = _questFactory.MakeNewQuest();
			newQuest.Start(CurrentTime);
			NewQuestEvent?.Invoke(newQuest);
			return newQuest;
		}

		protected IQuest CreateAndStartNewQuest(ConfigID<QuestConfig> questConfig)
		{
			var newQuest = _questFactory.MakeFromConfig(questConfig);
			newQuest.Start(CurrentTime);
			NewQuestEvent?.Invoke(newQuest);
			return newQuest;
		}
	}
}