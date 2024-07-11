using System.Collections.Generic;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Data
{
	public class QuestsDataContainer
	{
		public static QuestsDataContainer Empty => new();
		public readonly List<IQuestData> ActiveQuests = new();
	}
}