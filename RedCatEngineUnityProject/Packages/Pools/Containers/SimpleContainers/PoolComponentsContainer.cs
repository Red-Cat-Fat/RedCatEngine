using System;
using System.Collections.Generic;
using System.Linq;
using RedCatEngine.Pools.Containers.Creators.InstanceCreator;
using RedCatEngine.Pools.Containers.Enumerators;
using RedCatEngine.Pools.Pools;
using UnityEngine;

namespace RedCatEngine.Pools.Containers.SimpleContainers
{
	public class PoolComponentsContainer<TComponent>
		: PoolObjectsContainer,
			IPoolComponentsContainer<TComponent>
		where TComponent : IPooledObject
	{
		public PoolComponentsContainer(IInstanceCreator creator)
			: base(creator)
		{
		}

		public TComponent this[int currentSelectIndex]
			=> CreatedPooledObjects[currentSelectIndex].GameObject.GetComponent<TComponent>();

		public new IEnumerator<TComponent> GetEnumerator()
		{
			return new PooledObjectTypedEnumerator<TComponent>(
				CreatedPooledObjects.Select(pooledObject => pooledObject.GameObject.GetComponent<TComponent>())
			);
		}

		public TComponent InstantiateComponent(params object[] context)
		{
			var instantiate = Instantiate(context);
			return instantiate.GetComponent<TComponent>();
		}

		public TComponent InstantiateComponent(Vector3 position, params object[] context)
		{
			var instantiate = Instantiate(position, context);
			return instantiate.GetComponent<TComponent>();
		}

		public TComponent InstantiateComponentAsLastSibling(params object[] context)
		{
			var instantiate = InstantiateAsLastSibling(context);
			return instantiate.GetComponent<TComponent>();
		}

		public void KillAll(Action<TComponent> callbackBeforeKill)
		{
			base.KillAll(go =>
				{
					if (go.TryGetComponent<TComponent>(out var component))
						callbackBeforeKill?.Invoke(component);
				}
			);
		}
	}
}