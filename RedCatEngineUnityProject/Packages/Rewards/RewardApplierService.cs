using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Rewards.Base;
using UnityEngine;

namespace RedCatEngine.Rewards
{
	public class RewardApplierService : IRewardApplierService
	{
		private readonly IApplicationContainer _applicationContainer;

		[Inject]
		public RewardApplierService(IApplicationContainer applicationContainer)
		{
			_applicationContainer = applicationContainer;
		}

		public void Apply(IReward reward)
		{
			Debug.Log($"[RewardApplierService] Apply reward: {GetName(reward)}");
			reward.ApplyReward(_applicationContainer);
		}

		public string GetName(IReward reward)
			=> reward.GetName(_applicationContainer);
	}
}