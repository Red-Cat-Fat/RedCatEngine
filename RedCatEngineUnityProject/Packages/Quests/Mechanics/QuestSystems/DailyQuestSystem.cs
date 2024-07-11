using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Mechanics.QuestSystems
{
	public class DailyQuestSystem : BaseQuestSystem, IDisposable
	{
		private readonly int _countQuests;
		private readonly int _dailyTimeLiveSeconds;

		public DailyQuestSystem(
			QuestsDataContainer data,
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

		private void DoAfterLoadData()
		{
			CheckExpireQuestsAndUpdate();
			AddToNeedCountQuest();
		}

		private void OnChangeQuestState(ConfigID<QuestConfig> configID)
		{
			_activeQuest.RemoveAll(quest
				=> quest.QuestState is QuestState.Fail or QuestState.Finished);
			CheckExpireQuestsAndUpdate();
			UpdateSkipQuests();
		}

		private void UpdateSkipQuests()
		{
			for (var i = 0; i < _activeQuest.Count; i++)
			{
				if(_activeQuest[i].QuestState == QuestState.Skip)
					_activeQuest[i] = CreateAndStartNewQuest();
			}
		}

		private void CheckExpireQuestsAndUpdate()
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

		private void AddToNeedCountQuest()
		{
			for (var i = _activeQuest.Count; i < _countQuests; i++)
			{
				var newQuest = CreateAndStartNewQuest();
				newQuest.ChangeQuestStateEvent += OnChangeQuestState;
				_activeQuest.Add(newQuest);
			}
		}

		public void Dispose()
		{
			foreach (var quest in _activeQuest)
				quest.ChangeQuestStateEvent -= OnChangeQuestState;
		}
	}
}