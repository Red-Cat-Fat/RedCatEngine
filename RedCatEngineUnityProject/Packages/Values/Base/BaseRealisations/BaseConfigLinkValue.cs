using System;
using RedCatEngine.Values.Base.Interfaces;
using IGetterApplicationContainer =
	RedCatEngine.DependencyInjection.Containers.Interfaces.Application.IGetterApplicationContainer;

namespace RedCatEngine.Values.Base.BaseRealisations
{
	[Serializable]
	public class BaseConfigLinkValue<TValue> : IValue<TValue>
	{
		public BaseValueConfig<TValue> ValueConfig;

		public TValue GetValue(IGetterApplicationContainer getterContainer)
			=> ValueConfig.GetValue(getterContainer);
	}
}