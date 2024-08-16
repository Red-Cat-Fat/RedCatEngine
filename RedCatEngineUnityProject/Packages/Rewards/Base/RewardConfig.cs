using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Rewards.Base
{
	public abstract class RewardConfig : BaseConfig
	{
		[SR]
		[SerializeReference]
		public IReward Reward;

		public string GetName(IApplicationContainer applicationContainer)
			=> Reward.GetName(applicationContainer);

		public void ApplyReward(IApplicationContainer applicationContainer)
			=> Reward.ApplyReward(applicationContainer);
	}
}