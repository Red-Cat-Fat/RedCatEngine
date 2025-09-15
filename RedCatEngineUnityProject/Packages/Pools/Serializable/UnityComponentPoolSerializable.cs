using System;
using System.Collections;
using System.Collections.Generic;
using RedCatEngine.Pools.Containers;
using RedCatEngine.Pools.Containers.Creators.InstanceCreator;
using RedCatEngine.Pools.Containers.SimpleContainers;
using RedCatEngine.Pools.Pools;
using UnityEngine;

namespace RedCatEngine.Pools.Serializable
{
	/// <summary>
	/// Обёртка над пулом объектов в Unity, позволяющая управлять созданием и хранением экземпляров,
	/// реализующих интерфейс <see cref="IPooledObject"/>.
	/// </summary>
	/// <typeparam name="TComponent">Тип объекта, который должен реализовывать интерфейс <see cref="IPooledObject"/>.</typeparam>
	[Serializable]
	public class UnityComponentPoolSerializable<TComponent> : IPoolObjectsContainer, IPoolComponentsContainer<TComponent>
		where TComponent : IPooledObject
	{
		/// <summary>
		/// Префаб из которого создаются объекты в пул.
		/// </summary>
		[SerializeField] private GameObject _prefab;

		/// <summary>
		/// Родительский объект, к которому будут привязаны созданные объекты.
		/// </summary>
		[SerializeField] private Transform _parent;

		/// <summary>
		/// Получает родительский объект для позиционирования инстансов.
		/// </summary>
		public Transform Parent => _parent;

		/// <summary>
		/// Контейнер пула, в котором хранятся и управляются объекты типа <typeparamref name="TComponent"/>.
		/// </summary>
		private IPoolComponentsContainer<TComponent> _hiddenPool;

		/// <summary>
		/// Получает контейнер пула. При первом обращении инициализирует его, используя указанный префаб и родителя.
		/// </summary>
		private IPoolComponentsContainer<TComponent> SafeContainer =>
			_hiddenPool ??= new PoolComponentsContainer<TComponent>(
				new SimpleUnityInstanceCreator(_prefab, _parent)
			);

		public int Length 
			=> SafeContainer.Length;

		public void SetBeforeDisableCallback(Action<GameObject> callbackBeforeDisable) =>
			SafeContainer.SetBeforeDisableCallback(callbackBeforeDisable);

		public GameObject InstantiateAsLastSibling(params object[] context)
			=> SafeContainer.InstantiateAsLastSibling(context);

		public void SetAfterEnableCallback(Action<GameObject> callbackAfterEnable) =>
			SafeContainer.SetAfterEnableCallback(callbackAfterEnable);

		public TComponent this[int currentSelectIndex]
			=> SafeContainer[currentSelectIndex];

		public IEnumerator<TComponent> GetEnumerator()
			=> SafeContainer.GetEnumerator();

		public TComponent InstantiateComponent(params object[] context)
			=> SafeContainer.InstantiateComponent(context);

		public TComponent InstantiateComponent(Vector3 position, params object[] context) =>
			SafeContainer.InstantiateComponent(position, context);

		public TComponent InstantiateComponentAsLastSibling(params object[] context)
			=> SafeContainer.InstantiateComponentAsLastSibling(context);

		public void KillAll(Action<TComponent> callbackBeforeKill)
			=> SafeContainer.KillAll(callbackBeforeKill);

		public void KillAll(Action<GameObject> callbackBeforeKill = null)
			=> SafeContainer.KillAll(callbackBeforeKill);

		public GameObject Instantiate(params object[] context)
			=> SafeContainer.Instantiate(context);

		public GameObject Instantiate(Vector3 position, params object[] context) =>
			SafeContainer.Instantiate(position, context);

		public GameObject Instantiate(Vector3 position, Quaternion rotation, params object[] context) =>
			SafeContainer.Instantiate(position, rotation, context);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}