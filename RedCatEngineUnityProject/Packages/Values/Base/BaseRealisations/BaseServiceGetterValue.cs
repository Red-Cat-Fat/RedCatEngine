using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;

namespace RedCatEngine.Values.Base.BaseRealisations
{
	public abstract class BaseServiceGetterValue<TServiceType, TReturnedType> : IValue<TReturnedType>
	{
		public TReturnedType GetValue(IGetterApplicationContainer getterContainer)
			=> GetValueFromServices(getterContainer.GetSingle<TServiceType>());

		protected abstract TReturnedType GetValueFromServices(TServiceType singleService);
	}
}