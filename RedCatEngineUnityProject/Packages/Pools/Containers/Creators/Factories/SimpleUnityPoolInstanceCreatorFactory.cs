using RedCatEngine.Pools.Containers.Creators.InstanceCreator;
using UnityEngine;

namespace RedCatEngine.Pools.Containers.Creators.Factories
{
	public class SimpleUnityPoolInstanceCreatorFactory : IPoolInstanceCreatorFactory
	{
		public IInstanceCreator Make(GameObject prefab, Transform parent)
			=> new SimpleUnityInstanceCreator(prefab, parent);
	}
}