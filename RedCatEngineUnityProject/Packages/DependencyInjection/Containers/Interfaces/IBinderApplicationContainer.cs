using RedCatEngine.DependencyInjection.Specials.Providers;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces
{
	public interface IBinderApplicationContainer
	{
		IProvider<TProvideType> RegisterProvider<TProvideType>() where TProvideType : class;
		TBindType BindAsSingle<TBindType>(TBindType instance);
		TBindType BindAsArray<TBindType>(TBindType instance);
	}
}