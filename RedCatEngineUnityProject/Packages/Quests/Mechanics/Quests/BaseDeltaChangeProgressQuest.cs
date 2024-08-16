using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;
using RedCatEngine.Rewards.Base;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public abstract class BaseDeltaChangeProgressQuest : BaseSavedQuest<DeltaChangeProgressQuestData>
	{
		public override double Progress
			=> Math.Max(0, Math.Min(1, (CurrentDeltaValue - StartValue) / DeltaValue));
		public override string ProcessProgressText
			=> $"{Math.Min(Math.Max(0, CurrentDeltaValue - StartValue), DeltaValue)} / {DeltaValue}";

		protected double DeltaValue { get; }
		protected double CurrentDeltaValue { get; private set; }
		protected double StartValue { get; private set; }

		protected BaseDeltaChangeProgressQuest(
			ConfigID<QuestConfig> config,
			IReward reward,
			double deltaValue
		) : base(config, reward)
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

		protected void SetStartAndCurrentValue(double startValue)
		{
			StartValue = startValue;
			CurrentDeltaValue = startValue;
			CheckComplete();
		}

		protected void SetCurrentValue(double value)
		{
			CurrentDeltaValue = value;
			UpdateProgress();
			CheckComplete();
		}

		protected void IncrementCurrentValue()
			=> SetCurrentValue(CurrentDeltaValue + 1);

		protected void AddDeltaCurrentValue(double delta)
			=> SetCurrentValue(CurrentDeltaValue + delta);

		protected override void CheckComplete()
		{
			if(QuestState is not QuestState.InProgress)
				return;
			if (StartValue + DeltaValue <= CurrentDeltaValue)
				SendComplete();
		}
	}
}