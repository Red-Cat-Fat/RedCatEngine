using System;

namespace RedCatEngine.Quests.Mechanics.Quests.QuestDatas
{
	[Serializable]
	public class CollectProgressQuestData : BaseQuestData
	{
		public double CurrentValue;
		public override string ToString()
		{
			return $"{nameof(CollectProgressQuestData)}{base.ToString()} CurrentValue: {CurrentValue};";
		}
	}
}