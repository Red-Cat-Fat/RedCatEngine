using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Rewards.Base;

namespace RedCatEngine.Quests.Tests.SpecialSubClasses
{
	public class TestDeltaChangeProgressQuest : BaseDeltaChangeProgressQuest
	{
		public void SetStartValueForTest(float startValue)
			=> SetStartAndCurrentValue(startValue);

		public void SetCurrentValueForTest(float newCurrentValue)
			=> SetCurrentValue(newCurrentValue);

		protected override void DoResetValue() { }

		protected override void DoStart() { }

		protected override void DoClose() { }

		public override string GetDescription()
		{
			throw new System.NotImplementedException();
		}

		public TestDeltaChangeProgressQuest(
			ConfigID<QuestConfig> config,
			IReward reward,
			float deltaValue
		)
			: base(
				config,
				reward,
				deltaValue) { }
	}
}