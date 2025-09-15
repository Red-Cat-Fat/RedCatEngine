using System;
using System.Collections.Generic;
using System.Linq;
using RedCatEngine.Pools.Pools;
using UnityEngine;

namespace RedCatEngine.Pools.Objects
{
	public class ObjectComponentPull<TComponent> : ObjectsPool, IEnumerable<TComponent> where TComponent : IPooledObject
	{
		public ObjectComponentPull(GameObject prefab, Transform parentTransform)
			: base(prefab, parentTransform)
		{
		}

		public TComponent this[int currentSelectIndex]
			=> CreatedPooledObjects[currentSelectIndex].GameObject.GetComponent<TComponent>();

		public new IEnumerator<TComponent> GetEnumerator()
		{
			return new PooledObjectTypedEnumerator<TComponent>(
				CreatedPooledObjects.Select(pooledObject => pooledObject.GameObject.GetComponent<TComponent>()));
		}

		public TComponent InstantiateComponent()
		{
			Instantiate<TComponent>(out var component);
			return component;
		}

		public TComponent InstantiateComponentAsLastSibling()
		{
			InstantiateComponentAsLastSibling<TComponent>(out var component);
			return component;
		}

		public void KillAll(Action<TComponent> callbackBeforeKill)
		{
			base.KillAll(
				go =>
				{
					if (go.TryGetComponent<TComponent>(out var component))
						callbackBeforeKill?.Invoke(component);
				});
		}
	}
}