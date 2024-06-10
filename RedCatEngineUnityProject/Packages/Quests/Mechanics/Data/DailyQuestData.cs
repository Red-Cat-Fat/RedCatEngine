using System.Collections.Generic;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Data
{
	public class DailyQuestData
	{
		public static DailyQuestData Empty => new();
		public List<IQuestData> ActiveQuests = new();
	}
}