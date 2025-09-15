using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Tests.SpecialSubClasses
{
	public class TestCollectProgressQuest : BaseCollectProgressQuest
	{
		public TestCollectProgressQuest(
			ConfigID<QuestConfig> config,
			float targetValue
		)
			: base(
				config,
				targetValue)
		{
		}

		public void SetCurrentValueForTest(float newValue)
			=> SetCurrentValue(newValue);

		protected override void DoResetValue()
		{
		}

		protected override void DoStart()
		{
		}

		protected override void DoReset()
		{
		}

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