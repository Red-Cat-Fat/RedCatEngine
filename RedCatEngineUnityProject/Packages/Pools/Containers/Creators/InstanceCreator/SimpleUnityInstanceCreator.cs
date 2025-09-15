using RedCatEngine.Pools.Pools;
using UnityEngine;

namespace RedCatEngine.Pools.Containers.Creators.InstanceCreator
{
	public class SimpleUnityInstanceCreator : IInstanceCreator
	{
		private readonly GameObject _prefab;
		private readonly Transform _parent;

		public SimpleUnityInstanceCreator(GameObject prefab, Transform parent)
		{
			_prefab = prefab;
			_parent = parent;
		}

		public IPooledObject Create(params object[] context)
		{
			var instance = Object.Instantiate(_prefab, _parent);
			if (instance.TryGetComponent<IPooledObject>(out var pooledObject))
				return pooledObject;

			var programApplier = instance.AddComponent<CollectorComponentsPoolObject>();
			programApplier.CollectPooledComponents();

			return programApplier;
		}
	}
}