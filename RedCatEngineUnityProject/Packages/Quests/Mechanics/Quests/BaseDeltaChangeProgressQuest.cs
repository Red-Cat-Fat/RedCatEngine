using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;
using UnityEngine;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public abstract class BaseDeltaChangeProgressQuest : BaseSavedQuest<DeltaChangeProgressQuestData>
	{
		public override float Progress
			=> Mathf.Max(0, Mathf.Min(1, (CurrentDeltaValue - StartValue) / DeltaValue));
		public override string ProcessProgressText
			=> $"{Mathf.Min((CurrentDeltaValue - StartValue), DeltaValue)} / {DeltaValue}";

		private float DeltaValue { get; }
		private float CurrentDeltaValue { get; set; }
		private float StartValue { get; set; }

		protected BaseDeltaChangeProgressQuest(ConfigID<QuestConfig> config, float deltaValue) : base(config)
		{
			DeltaValue = deltaValue;
		}

		protected sealed override IQuest DoLoadData(DeltaChangeProgressQuestData questData)
		{
			CurrentDeltaValue = questData.CurrentDeltaValue;
			StartValue = questData.StartValue;
			return this;
		}

		protected sealed override DeltaChangeProgressQuestData DoSaveData(DeltaChangeProgressQuestData saveData)
		{
			saveData.StartValue = StartValue;
			saveData.CurrentDeltaValue = CurrentDeltaValue;
			return saveData;
		}

		protected void SetStartValue(float startValue)
			=> StartValue = startValue;

		protected void SetCurrentValue(float value)
			=> CurrentDeltaValue = value;

		protected void IncrementCurrentValue()
		{
			SetCurrentValue(CurrentDeltaValue + 1);
			if (StartValue + DeltaValue <= CurrentDeltaValue)
				SendComplete();
		}

		protected void AddCurrentValue(float delta)
		{
			SetCurrentValue(CurrentDeltaValue + delta);
			if (StartValue + DeltaValue <= CurrentDeltaValue)
				SendComplete();
		}
	}
}