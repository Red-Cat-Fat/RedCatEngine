using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;
using UnityEngine;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public abstract class BaseCollectProgressQuest : BaseSavedQuest<CollectProgressQuestData>
	{
		public override float Progress => Mathf.Min(1, CurrentValue / TargetValue);
		public override string ProcessProgressText => $"{Mathf.Min(CurrentValue, TargetValue)} / {TargetValue}";

		private float TargetValue { get; }
		private float CurrentValue { get; set; }

		protected BaseCollectProgressQuest(ConfigID<QuestConfig> config, float targetValue) : base(config)
		{
			TargetValue = targetValue;
		}

		protected void SetCurrentValue(float value)
		{
			CurrentValue = value;

			if (TargetValue <= CurrentValue)
				SendComplete();
		}

		protected void IncrementCurrentValue() 
			=> SetCurrentValue(CurrentValue + 1);

		protected void AddCurrentValue(float delta)
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