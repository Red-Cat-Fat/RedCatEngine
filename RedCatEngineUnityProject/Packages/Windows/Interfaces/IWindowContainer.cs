using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;

namespace RedCatEngine.Windows.Interfaces
{
	public interface IWindowContainer : IGetterApplicationContainer, ICreator, IMonoCreator
	{
	}
}