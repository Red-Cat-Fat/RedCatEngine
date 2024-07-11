using System.Collections.Generic;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Data
{
	public class DailyQuestsData
	{
		public static DailyQuestsData Empty => new();
		public List<IQuestData> ActiveQuests = new();
	}
}