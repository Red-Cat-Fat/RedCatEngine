using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.Binders;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Application
{
	public interface IApplicationContainer :
		IBinderApplicationContainer,
		ITypeBinderApplicationContainer,
		IGetterApplicationContainer,
		ICreator
	{
	}
}