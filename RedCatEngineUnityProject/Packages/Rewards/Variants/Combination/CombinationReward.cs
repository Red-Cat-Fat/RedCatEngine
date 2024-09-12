using System;
using System.Text;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Rewards.Base;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Rewards.Variants.Combination
{
	[Serializable]
	[SRName("Base/Combination Reward")]
	public class CombinationReward : IReward
	{
		[SR]
		[SerializeReference]
		public IReward[] Rewards;

		public string GetName(IApplicationContainer applicationContainer)
		{
			var sb = new StringBuilder();
			for (var index = 0; index < Rewards.Length; index++)
			{
				sb.Append(Rewards[index].GetName(applicationContainer));
				if (index != Rewards.Length)
					sb.Append(",");
			}

			return sb.ToString();
		}

		public void ApplyReward(IApplicationContainer applicationContainer)
		{
			foreach (var reward in Rewards)
				reward.ApplyReward(applicationContainer);
		}
	}
}