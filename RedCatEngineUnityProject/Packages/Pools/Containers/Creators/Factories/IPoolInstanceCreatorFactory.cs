using RedCatEngine.Pools.Containers.Creators.InstanceCreator;
using UnityEngine;

namespace RedCatEngine.Pools.Containers.Creators.Factories
{
	public interface IPoolInstanceCreatorFactory
	{
		IInstanceCreator Make(GameObject prefab, Transform parent);
	}
}