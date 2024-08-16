using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.Rewards.Base;
using SerializeReferenceEditor;

namespace RedCatEngine.Rewards.Variants
{
	[Serializable]
	[SRName("Base/Config link")]
	public class RewardConfigLink : IReward
	{
		public RewardConfig RewardConfig;

		public string GetName(IApplicationContainer applicationContainer)
			=> RewardConfig.GetName(applicationContainer);

		public void ApplyReward(IApplicationContainer applicationContainer)
		{
			RewardConfig.ApplyReward(applicationContainer);
		}
	}
}