using RedCatEngine.DependencyInjection.Specials.Providers;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces
{
	public interface IArrayBinderApplicationContainer
	{
		ISingleProvider<TProvideType> RegisterArrayProvider<TProvideType>() where TProvideType : class;
		TBindType BindAsArray<TBindType>(TBindType instance);
	}
}