using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;

namespace RedCatEngine.Rewards.Base
{
	public interface IReward
	{
		string GetName(IApplicationContainer applicationContainer);
		void ApplyReward(IApplicationContainer applicationContainer);

		static IReward Empty
			=> new EmptyReward();
	}
}