using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;
using RedCatEngine.Pools.Containers.Creators.InstanceCreator;
using RedCatEngine.Pools.Pools;
using UnityEngine;

namespace RedCatEngine.CommonServices.Factories
{
	public class DiInstanceCreator : IInstanceCreator
	{
		private readonly IUnityGameContainer _container;
		private readonly GameObject _prefab;
		private readonly Transform _parent;

		public DiInstanceCreator(
			IUnityGameContainer container,
			GameObject prefab,
			Transform parent
		)
		{
			_container = container;
			_prefab = prefab;
			_parent = parent;
		}

		public IPooledObject Create(params object[] context)
		{
			var instance = _container.Create(
				_prefab,
				_parent,
				context: context);
			if (instance.TryGetComponent<IPooledObject>(out var pooledObject))
				return pooledObject;

			var collector = instance.AddComponent<CollectorComponentsPoolObject>();
			collector.CollectPooledComponents();
			return collector;
		}
	}
}