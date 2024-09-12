using System;
using RedCatEngine.Conditions;
using RedCatEngine.Conditions.Base;
using RedCatEngine.Conditions.Variants;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Rewards.Base;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Rewards.Variants
{
	[Serializable]
	[SRName("Base/Condition Reward")]
	public class ConditionReward : IReward
	{
		[SR]
		[SerializeReference]
		public ICondition Condition = ForceCondition.True;

		[SR]
		[SerializeReference]
		public IReward Reward;

		[SR]
		[SerializeReference]
		public IReward AlternativeReward;

		public string GetName(IApplicationContainer applicationContainer)
		{
			var result = IsApplyBaseReward(applicationContainer);
			return result ? Reward.GetName(applicationContainer) : AlternativeReward.GetName(applicationContainer);
		}

		public void ApplyReward(IApplicationContainer applicationContainer)
		{
			var result = IsApplyBaseReward(applicationContainer);

			if (applicationContainer.TryGetSingle<IRewardApplierService>(out var rewardApplierService))
			{
				rewardApplierService.Apply(result ? Reward : AlternativeReward);
			}
			else
			{
				Debug.LogWarning("Apply Reward without RewardApplierService");
				if (result)
					Reward.ApplyReward(applicationContainer);
				else
					AlternativeReward.ApplyReward(applicationContainer);
			}
		}

		private bool IsApplyBaseReward(IApplicationContainer applicationContainer)
		{
			bool result;
			if (applicationContainer.TryGetSingle<IConditionCheckerService>(out var conditionCheckerService))
			{
				result = conditionCheckerService.Check(Condition);
			}
			else
			{
				Debug.LogWarning("Apply Reward without ConditionCheckerService");
				result = Condition.Check(applicationContainer);
			}

			return result;
		}
	}
}