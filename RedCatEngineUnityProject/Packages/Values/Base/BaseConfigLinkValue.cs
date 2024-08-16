using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces;

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