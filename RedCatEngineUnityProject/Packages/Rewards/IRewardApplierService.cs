using RedCatEngine.Rewards.Base;

namespace RedCatEngine.Rewards
{
	public interface IRewardApplierService
	{
		void Apply(IReward reward);
		string GetName(IReward reward);
	}
}