using System;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Mechanics.QuestSystems
{
	public class DailyQuestSystem : BaseQuestSystem
	{
		private readonly int _countQuests;
		private readonly int _dailyTimeLiveSeconds;

		public DailyQuestSystem(
			DailyQuestsData data,
			IQuestFactory questFactory,
			int countQuests,
			int dailyTimeLiveSeconds
		)
			: base(questFactory)
		{
			_countQuests = countQuests;
			_dailyTimeLiveSeconds = dailyTimeLiveSeconds;
			LoadData(data);
			DoAfterLoadData();
		}

		private void AddToNeedCountQuest()
		{
			for (var i = _activeQuest.Count; i < _countQuests; i++)
				_activeQuest.Add(CreateAndStartNewQuest());
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

		private void DoAfterLoadData()
		{
			CheckExpireQuests();
			AddToNeedCountQuest();
		}
	}
}