using System;
using System.Collections.Generic;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Mechanics.QuestSystems
{
	public abstract class BaseQuestSystem : IDisposable
	{
		public event Action<IQuest> ChangeStateQuest;
		public event Action<IQuest> NewQuestEvent;
		protected readonly List<IQuest> ActiveQuests = new();

		protected static DateTime CurrentTime
			=> DateTime.UtcNow; //todo: make time service

		protected readonly IQuestFactory QuestFactory;

		protected BaseQuestSystem(IQuestFactory questFactory)
		{
			QuestFactory = questFactory;
		}

		public void LoadData(QuestsDataContainer questsDataContainer)
		{
			if (questsDataContainer == QuestsDataContainer.Empty)
			{
				AfterLoadData();
				return;
			}

			Clear();
			var savedQuest = questsDataContainer.GetQuests();
			foreach (var questData in savedQuest)
			{
				var loadQuest = QuestFactory.LoadFrom(questData);
				if (loadQuest == null)
					continue;

				loadQuest.Continue();
				ActiveQuests.Add(loadQuest);
			}

			AfterLoadData();
		}

		private void AfterLoadData()
		{
			DoAfterLoadData();
			foreach (var quest in ActiveQuests)
				quest.ChangeQuestStateEvent += OnChangeQuestState;
		}

		public QuestsDataContainer GetData()
		{
			var data = new QuestsDataContainer();
			foreach (var quest in ActiveQuests)
				data.AddQuest(quest.GetData());

			return data;
		}

		private bool TryGetQuest(ConfigID<QuestConfig> config, out IQuest quest)
		{
			foreach (var activeQuest in ActiveQuests)
			{
				if (activeQuest.Config != config)
					continue;

				quest = activeQuest;
				return true;
			}

			quest = null;
			return false;
		}

		public List<IQuest> GetActiveQuest()
			=> ActiveQuests;

		protected IQuest CreateAndStartNewQuest()
		{
			var newQuest = QuestFactory.MakeNewQuest(ActiveQuests);
			newQuest.Start(CurrentTime);
			NewQuestEvent?.Invoke(newQuest);
			return newQuest;
		}

		protected IQuest CreateAndStartNewQuest(ConfigID<QuestConfig> questConfig)
		{
			var newQuest = QuestFactory.MakeFromConfig(questConfig);
			newQuest.Start(CurrentTime);
			NewQuestEvent?.Invoke(newQuest);
			return newQuest;
		}

		private void Clear()
		{
			DoClear();
			ActiveQuests.Clear();
		}

		protected abstract void DoClear();

		public void Dispose()
		{
			foreach (var quest in ActiveQuests)
				quest.ChangeQuestStateEvent -= OnChangeQuestState;
			Clear();
		}

		private void OnChangeQuestState(IQuest quest)
		{
			ChangeStateQuest?.Invoke(quest);
		}
		
		protected abstract void DoAfterLoadData();
	}
}