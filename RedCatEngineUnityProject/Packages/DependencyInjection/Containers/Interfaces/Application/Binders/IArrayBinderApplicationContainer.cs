using System;
using RedCatEngine.DependencyInjection.Specials.Providers;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Application.Binders
{
	public interface IArrayBinderApplicationContainer
	{
		IArrayProvider<TProvideType> RegisterArrayProvider<TProvideType>() where TProvideType : class;
		object RegisterArrayProvider(Type providerType);
		TBindType BindAsArray<TBindType>(TBindType instance);
	}
}