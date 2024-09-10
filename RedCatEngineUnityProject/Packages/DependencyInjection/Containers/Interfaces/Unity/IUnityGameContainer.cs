using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Unity
{
	public interface
		IUnityGameContainer : IApplicationContainer, IMonoConstructor, IMonoBindInstance, IMonoCreator { }
}