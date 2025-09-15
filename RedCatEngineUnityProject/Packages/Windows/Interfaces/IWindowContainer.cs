using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;

namespace Infrastructure.Windows.Interfaces
{
	public interface IWindowContainer : IGetterApplicationContainer, ICreator, IMonoCreator
	{
		public IModel FillContextToModel(IModel model, params object[] context);
	}
}