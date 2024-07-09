using System;
using System.Collections.Generic;
using System.Linq;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Mechanics
{
	public class DailyQuestSystem
	{
		public event Action<IQuest> NewQuestEvent;
		private readonly IQuestFactory _questFactory;
		private readonly int _countQuests;
		private readonly int _dailyTimeLiveSeconds;
		private readonly List<IQuest> _activeQuest = new();
		private static DateTime CurrentTime => DateTime.UtcNow; //todo: make time service

		public DailyQuestSystem(
			DailyQuestsData data,
			IQuestFactory questFactory,
			int countQuests,
			int dailyTimeLiveSeconds
		)
		{
			_questFactory = questFactory;
			_countQuests = countQuests;
			_dailyTimeLiveSeconds = dailyTimeLiveSeconds;
			LoadData(data);
		}

		private void LoadData(DailyQuestsData dailyQuestsData)
		{
			_activeQuest.AddRange(
				dailyQuestsData.ActiveQuests
					.Select(questData => _questFactory.LoadFrom(questData))
					.Where(quest => quest != null));
			CheckExpireQuests();
			AddToNeedCountQuest();
		}

		private void AddToNeedCountQuest()
		{
			for (var i = _activeQuest.Count; i < _countQuests; i++)
				_activeQuest.Add(CreateAndStartNewQuest());
		}

		public DailyQuestsData GetData()
		{
			var data = new DailyQuestsData();
			foreach (var quest in _activeQuest)
				data.ActiveQuests.Add(quest.GetData());

			return data;
		}

		public List<IQuest> GetActiveQuest()
		{
			return _activeQuest;
		}

		private void CheckExpireQuests()
		{
			var currentTime = CurrentTime;
			for (var i = 0; i < _activeQuest.Count; i++)
			{
				var checkQuest = _activeQuest[i];
				if (!QuestIsExpireTime(currentTime, checkQuest))
					continue;
				_activeQuest[i] = CreateAndStartNewQuest();
			}
		}

		private bool QuestIsExpireTime(DateTime currentTime, IQuest checkQuest)
		{
			var questData = checkQuest.GetData();
			return (currentTime - questData.CreateTime).TotalSeconds > _dailyTimeLiveSeconds;
		}

		private IQuest CreateAndStartNewQuest()
		{
			var newQuest = _questFactory.MakeNewQuest();
			newQuest.Start(CurrentTime);
			NewQuestEvent?.Invoke(newQuest);
			return newQuest;
		}
	}
}