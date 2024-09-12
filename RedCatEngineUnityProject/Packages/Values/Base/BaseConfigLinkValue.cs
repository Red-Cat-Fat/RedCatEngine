using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;

namespace RedCatEngine.Values.Base
{
	[Serializable]
	public class BaseConfigLinkValue<TValue> : IValue<TValue>
	{
		public BaseValueConfig<TValue> ValueConfig;
		public TValue GetValue(IApplicationContainer applicationContainer)
			=> ValueConfig.GetValue(applicationContainer);
	}
}