using System;

namespace RedCatEngine.Quests.Mechanics.Quests.QuestDatas
{
	[Serializable]
	public class DeltaChangeProgressQuestData : BaseQuestData
	{
		public double CurrentDeltaValue;
		public double StartValue;
		public override string ToString()
		{
			return $"{nameof(DeltaChangeProgressQuestData)}{base.ToString()} CurrentDeltaValue: {CurrentDeltaValue}; StartValue: {StartValue}";
		}
	}
}