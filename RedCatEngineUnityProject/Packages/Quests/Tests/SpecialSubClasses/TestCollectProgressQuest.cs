using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Rewards.Base;

namespace RedCatEngine.Quests.Tests.SpecialSubClasses
{
	public class TestCollectProgressQuest : BaseCollectProgressQuest
	{

		public void SetCurrentValueForTest(float newValue)
			=> SetCurrentValue(newValue);

		protected override void DoResetValue()
		{
			
		}

		protected override void DoStart()
		{
			
		}

		protected override void DoClose()
		{
			
		}

		public override string GetDescription()
		{
			throw new System.NotImplementedException();
		}

		public TestCollectProgressQuest(ConfigID<QuestConfig> config, IReward reward,
			float targetValue
		)
			: base(config, reward,
			       targetValue) { }
	}
}