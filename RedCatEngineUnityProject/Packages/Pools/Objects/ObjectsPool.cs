using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RedCatEngine.Pools.Objects.Enumerators;
using RedCatEngine.Pools.Pools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RedCatEngine.Pools.Objects
{
	public class ObjectsPool : IObjectsPool, IEnumerable
	{
		private readonly Stack<GameObject> _inactiveStack = new();
		private readonly Transform _parentTransform;
		private readonly GameObject _prefab;
		protected readonly List<IPooledObject> CreatedPooledObjects = new();

		public ObjectsPool(GameObject prefab, Transform parentTransform)
		{
			_prefab = prefab;
			_parentTransform = parentTransform;
			if (!_prefab.TryGetComponent<IPooledObject>(out _))
				throw new Exception("Object not contain pooled component");
		}

		public int Length
			=> CreatedPooledObjects.Count;

		public virtual IEnumerator GetEnumerator()
			=> new GameObjectsEnumerator(CreatedPooledObjects.Select(pool => pool.GameObject));

		public GameObject Instantiate<TComponent>(out TComponent component)
			where TComponent : IPooledObject
		{
			GameObject instance;
			if (_inactiveStack.Count > 0)
			{
				instance = _inactiveStack.Pop();
				component = instance.GetComponent<TComponent>();
				component.Reset();
			}
			else
			{
				instance = Object.Instantiate(_prefab, _parentTransform);
				component = instance.GetComponent<TComponent>();
			}

			component.DeadEvent += OnDead;
			component.Enable();

			if (!CreatedPooledObjects.Contains(component))
				CreatedPooledObjects.Add(component);

			return instance;
		}

		public void OnDead(IPooledObject pooledObject)
		{
			if (CreatedPooledObjects.Contains(pooledObject))
				CreatedPooledObjects.Remove(pooledObject);

			pooledObject.DeadEvent -= OnDead;
			_inactiveStack.Push(pooledObject.GameObject);
		}

		public void KillAll(Action<GameObject> callbackBeforeKill = null)
		{
			var countCreatedObjects = CreatedPooledObjects.Count;
			var countRemove = 0;
			while (countCreatedObjects > countRemove && CreatedPooledObjects.Count > 0)
			{
				var createdObjectForRemove = CreatedPooledObjects[0];
				callbackBeforeKill?.Invoke(createdObjectForRemove.GameObject);
				createdObjectForRemove.Disable();
				countRemove++;
			}
		}

		public GameObject InstantiateComponentAsLastSibling<TComponent>(out TComponent component)
			where TComponent : IPooledObject
		{
			GameObject instance = Instantiate(out component);
			instance.transform.SetAsLastSibling();
			return instance;
		}

		public GameObject Instantiate<TComponent>(
			Vector3 position,
			Quaternion rotation,
			out TComponent component
		)
			where TComponent : IPooledObject
		{
			var instance = Instantiate(out component);
			instance.transform.position = position;
			instance.transform.rotation = rotation;
			return instance;
		}

		public GameObject Instantiate(
			Vector3 position,
			Quaternion rotation,
			out IPooledObject component
		)
		{
			return Instantiate<IPooledObject>(
				position,
				rotation,
				out component);
		}

		public GameObject Instantiate<TComponent>(
			Vector2 position,
			Quaternion rotation,
			out TComponent component
		)
			where TComponent : IPooledObject
		{
			var instance = Instantiate(out component);
			instance.transform.position = position;
			instance.transform.rotation = rotation;
			return instance;
		}
	}
}