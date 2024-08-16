using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;
using RedCatEngine.Rewards.Base;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public abstract class BaseCollectProgressQuest : BaseSavedQuest<CollectProgressQuestData>
	{
		public override double Progress
			=> Math.Min(1, CurrentValue / TargetValue);

		public override string ProcessProgressText
			=> $"{Math.Min(CurrentValue, TargetValue)} / {TargetValue}";

		protected double TargetValue { get; }
		protected double CurrentValue { get; private set; }

		protected BaseCollectProgressQuest(ConfigID<QuestConfig> config, IReward reward,  double targetValue)
			: base(config, reward)
		{
			TargetValue = targetValue;
		}

		protected void SetCurrentValue(double value)
		{
			CurrentValue = value;

			CheckComplete();
			UpdateProgress();
		}

		protected override void CheckComplete()
		{
			if(QuestState != QuestState.InProgress)
				return;

			if (TargetValue <= CurrentValue)
				SendComplete();
		}

		protected void IncrementCurrentValue()
			=> SetCurrentValue(CurrentValue + 1);

		protected void AddDeltaCurrentValue(double delta)
			=> SetCurrentValue(CurrentValue + delta);

		protected override IQuest DoLoadData(CollectProgressQuestData questData)
		{
			CurrentValue = questData.CurrentValue;
			return this;
		}

		protected override CollectProgressQuestData DoSaveData(CollectProgressQuestData saveData)
		{
			saveData.CurrentValue = CurrentValue;
			return saveData;
		}
	}
}