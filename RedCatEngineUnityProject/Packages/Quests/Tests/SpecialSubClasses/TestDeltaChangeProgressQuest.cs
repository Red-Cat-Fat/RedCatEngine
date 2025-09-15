using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Tests.SpecialSubClasses
{
	public class TestDeltaChangeProgressQuest : BaseDeltaChangeProgressQuest
	{
		public TestDeltaChangeProgressQuest(
			ConfigID<QuestConfig> config,
			float deltaValue
		)
			: base(
				config,
				deltaValue)
		{
		}

		public void SetStartValueForTest(float startValue)
			=> SetStartAndCurrentValue(startValue);

		public void SetCurrentValueForTest(float newCurrentValue)
			=> SetCurrentValue(newCurrentValue);

		protected override void DoResetValue() { }

		protected override void DoStart() { }

		protected override void DoReset() { }

		protected override void DoSuccessFinished()
		{
		}

		public override string GetLocalizedName()
		{
			throw new System.NotImplementedException();
		}

		public override string GetLocalizedDescription()
		{
			throw new System.NotImplementedException();
		}
	}
}