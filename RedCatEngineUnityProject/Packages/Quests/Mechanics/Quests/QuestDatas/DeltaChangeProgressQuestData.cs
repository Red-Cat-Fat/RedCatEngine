using System;

namespace RedCatEngine.Quests.Mechanics.Quests.QuestDatas
{
	[Serializable]
	public class DeltaChangeProgressQuestData : BaseQuestData
	{
		public float CurrentDeltaValue;
		public float StartValue;
	}
}