using System;
using System.Linq;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Mechanics.QuestSystems
{
	public class DailyQuestSystem : BaseQuestSystem
	{
		private readonly int _countQuests;
		private readonly int _dailyTimeLiveSeconds;

		public DailyQuestSystem(
			IQuestFactory questFactory,
			int countQuests,
			int dailyTimeLiveSeconds
		)
			: base(questFactory)
		{
			_countQuests = countQuests;
			_dailyTimeLiveSeconds = dailyTimeLiveSeconds;
		}

		protected override void DoAfterLoadData()
		{
			SubscribeAfterLoadQuests();
			UpdateQuestList();
		}

		private void SubscribeAfterLoadQuests()
		{
			foreach (var quest in ActiveQuests)
				quest.ChangeQuestStateEvent += OnChangeQuestState;
		}

		private void OnChangeQuestState(IQuest quest)
		{
			UpdateQuestList();
		}

		private void UpdateQuestList()
		{
			var currentTime = CurrentTime;

			Func<IQuest, bool> PredicateForRemoveQuests()
				=> quest => IsNeedChangeQuest(quest) || QuestIsExpireTime(currentTime, quest);

			var toRemove = ActiveQuests.Where(PredicateForRemoveQuests()).ToArray();
			foreach (var quest in toRemove)
			{
				quest.Close();
				quest.ChangeQuestStateEvent -= OnChangeQuestState;
				ActiveQuests.Remove(quest);
			}

			for (var i = ActiveQuests.Count; i < _countQuests; i++)
				AddNewQuest();
		}

		private static bool IsNeedChangeQuest(IQuest quest)
			=> quest.QuestState is QuestState.Fail or QuestState.Finished or QuestState.Skip;

		private bool QuestIsExpireTime(DateTime currentTime, IQuest checkQuest)
		{
			var questData = checkQuest.GetData();
			return (currentTime - questData.GetCreateTime()).TotalSeconds > _dailyTimeLiveSeconds;
		}

		protected override void DoClear()
		{
			foreach (var quest in ActiveQuests)
				quest.ChangeQuestStateEvent -= OnChangeQuestState;
		}

		private void AddNewQuest()
		{
			var newQuest = CreateAndStartNewQuest();
			newQuest.ChangeQuestStateEvent += OnChangeQuestState;
			ActiveQuests.Add(newQuest);
		}
	}
}