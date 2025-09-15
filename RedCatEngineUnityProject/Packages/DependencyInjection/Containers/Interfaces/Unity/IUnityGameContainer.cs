using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using UnityEngine;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Unity
{
	public interface
		IUnityGameContainer : IApplicationContainer, IMonoConstructCreator, IMonoBindInstance, IMonoCreator
	{
		new IUnityGameContainer CreateChildContainer();
		TMonoBehaviorType CreateAndBindComponentAsSingle<TMonoBehaviorType>(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		) where TMonoBehaviorType : Component;
	}
}