using System;
using RedCatEngine.DependencyInjection.Specials.Providers;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Application.Binders
{
	public interface ISingleBinderApplicationContainer
	{
		ISingleProvider<TProvideType> RegisterProvider<TProvideType>() where TProvideType : class;
		object RegisterProvider(Type providerType);
		TBindType BindAsSingle<TBindType>(TBindType instance);
	}
}