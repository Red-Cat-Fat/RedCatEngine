﻿using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using SerializeReferenceEditor;

namespace RedCatEngine.Rewards.Base
{
	[SRName("Empty")]
	public class EmptyReward : IReward
	{
		public string GetName(IApplicationContainer applicationContainer)
			=> string.Empty;

		public void ApplyReward(IApplicationContainer applicationContainer)
		{
		}
	}
}