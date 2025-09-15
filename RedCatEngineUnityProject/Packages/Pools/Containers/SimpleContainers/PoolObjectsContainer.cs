using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RedCatEngine.Pools.Containers.Creators.InstanceCreator;
using RedCatEngine.Pools.Containers.Enumerators;
using RedCatEngine.Pools.Pools;
using UnityEngine;

namespace RedCatEngine.Pools.Containers.SimpleContainers
{
	public class PoolObjectsContainer : IPoolObjectsContainer, IEnumerable
	{
		private readonly IInstanceCreator _creator;
		private readonly Stack<IPooledObject> _inactiveStack = new();
		protected readonly List<IPooledObject> CreatedPooledObjects = new();
		private Action<GameObject> _callbackBeforeKill;
		private Action<GameObject> _callbackAfterEnable;

		public int Length
			=> CreatedPooledObjects.Count;

		public PoolObjectsContainer(
			IInstanceCreator creator
		)
		{
			_creator = creator;
		}

		public virtual IEnumerator GetEnumerator()
			=> new GameObjectsEnumerator(CreatedPooledObjects.Select(pool => pool.GameObject));

		private void OnDead(IPooledObject pooledObject)
		{
			_callbackBeforeKill?.Invoke(pooledObject.GameObject);

			if (CreatedPooledObjects.Contains(pooledObject))
				CreatedPooledObjects.Remove(pooledObject);

			pooledObject.DeadEvent -= OnDead;
			_inactiveStack.Push(pooledObject);
		}

		public void SetAfterEnableCallback(Action<GameObject> callbackAfterEnable)
		{
			_callbackAfterEnable = callbackAfterEnable;
		}

		public void KillAll([CanBeNull] Action<GameObject> callbackBeforeKill = null)
		{
			var countCreatedObjects = CreatedPooledObjects.Count;
			var countRemove = 0;
			while (countCreatedObjects > countRemove
					&& CreatedPooledObjects.Count > 0)
			{
				var createdObjectForRemove = CreatedPooledObjects[0];
				callbackBeforeKill?.Invoke(createdObjectForRemove.GameObject);
				createdObjectForRemove.Disable();
				countRemove++;
			}
		}

		public void SetBeforeDisableCallback(Action<GameObject> callbackBeforeDisable)
		{
			_callbackBeforeKill = callbackBeforeDisable;
		}

		public GameObject InstantiateAsLastSibling(params object[] context)
		{
			var instance = Instantiate(context);
			instance.transform.SetAsLastSibling();
			return instance;
		}

		public GameObject Instantiate(params object[] context)
		{
			IPooledObject pooledComponent;
			if (_inactiveStack.Count > 0)
			{
				pooledComponent = _inactiveStack.Pop();
				pooledComponent.Reset();
			}
			else
			{
				pooledComponent = _creator.Create(context);
			}

			pooledComponent.DeadEvent += OnDead;
			pooledComponent.Enable();

			if (!CreatedPooledObjects.Contains(pooledComponent))
				CreatedPooledObjects.Add(pooledComponent);

			_callbackAfterEnable?.Invoke(pooledComponent.GameObject);
			return pooledComponent.GameObject;
		}

		public GameObject Instantiate(
			Vector3 position,
			params object[] context
		)
		{
			return Instantiate(position, Quaternion.identity, context);
		}

		public GameObject Instantiate(
			Vector3 position,
			Quaternion rotation,
			params object[] context
		)
		{
			var instance = Instantiate(context);
			var pooledObject = instance.GetComponent<IPooledObject>();
			pooledObject.TeleportTo(position, rotation);
			return instance;
		}
	}
}