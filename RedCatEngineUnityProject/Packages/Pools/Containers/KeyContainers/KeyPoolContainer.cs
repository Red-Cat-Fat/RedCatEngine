using System;
using System.Collections.Generic;
using RedCatEngine.Pools.Containers.Creators.Factories;
using RedCatEngine.Pools.Containers.KeyContainers.Rules;
using RedCatEngine.Pools.Containers.SimpleContainers;
using RedCatEngine.Pools.Pools;
using UnityEngine;

namespace RedCatEngine.Pools.Containers.KeyContainers
{
	/// <summary>
	/// Контейнер пула игровых объектов, разделённых по ключам.
	/// Позволяет создавать, управлять и уничтожать игровые объекты на основе ключа <typeparamref name="TKey"/>.
	/// </summary>
	/// <typeparam name="TKey">Тип ключа, используемого для группировки объектов в пуле.
	///     Должен реализовывать интерфейс <see cref="IKeyRuleGameObjectSelector"/>.</typeparam>
	public class KeyPoolContainer<TKey> : IKeyPoolContainer<TKey> where TKey : IKeyRuleGameObjectSelector
	{
		/// <summary>
		/// Фабрика для создания инстансов игровых объектов.
		/// </summary>
		private readonly IPoolInstanceCreatorFactory _creatorFactory;

		/// <summary>
		/// Родительский трансформ, к которому будут добавляться игровые объекты.
		/// </summary>
		private readonly Transform _parentTransform;

		/// <summary>
		/// Словарь, хранящий пул объектов, индексированный по ключу <typeparamref name="TKey"/>.
		/// </summary>
		private readonly Dictionary<TKey, PoolObjectsContainer> _pools = new();

		/// <summary>
		/// Коллбэк, вызываемый после активации игрового объекта из пула.
		/// </summary>
		private Action<GameObject> _callbackAfterEnable;

		/// <summary>
		/// Коллбэк, вызываемый перед деактивацией игрового объекта в пуле.
		/// </summary>
		private Action<GameObject> _callbackBeforeDisable;

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="KeyPoolContainer{TKey}"/>.
		/// </summary>
		/// <param name="creatorFactory">Фабрика для создания инстансов игровых объектов.</param>
		/// <param name="parentTransform">Родительский трансформ для позиционирования объектов.</param>
		public KeyPoolContainer(IPoolInstanceCreatorFactory creatorFactory, Transform parentTransform)
		{
			_creatorFactory = creatorFactory;
			_parentTransform = parentTransform;
		}

		/// <summary>
		/// Получает или создаёт при отсутствии пул игровых объектов для указанного ключа.
		/// </summary>
		/// <param name="key">Ключ, определяющий тип объекта в пуле.</param>
		/// <returns>Пул объектов, связанных с данным ключом.</returns>
		private PoolObjectsContainer GetOrCreatePoolObject(TKey key)
		{
			if (_pools.TryGetValue(key, out var pool))
				return pool;

			var creator = _creatorFactory.Make(key.GetGameObjectForPool(), _parentTransform);
			pool = new PoolObjectsContainer(creator);
			pool.SetAfterEnableCallback(_callbackAfterEnable);
			pool.SetBeforeDisableCallback(_callbackBeforeDisable);
			_pools.Add(key, pool);
			return pool;
		}

		/// <summary>
		/// Создаёт и возвращает новый игровой объект из пула, соответствующего указанному ключу.
		/// </summary>
		/// <param name="key">Ключ, определяющий тип объекта в пуле.</param>
		/// <param name="position">Позиция, в которой будет создан объект.</param>
		/// <param name="rotation">Поворот, в котором будет создан объект.</param>
		/// <param name="additionalContext">Дополнительные параметры для передачи в конструктор объекта.</param>
		/// <returns>Созданный игровой объект.</returns>
		public GameObject Instantiate(
			TKey key,
			Vector3 position,
			Quaternion rotation,
			params object[] additionalContext
		)
		{
			var pool = GetOrCreatePoolObject(key);
			return pool.Instantiate(position, rotation, additionalContext);
		}

		/// <summary>
		/// Устанавливает коллбэк, который будет вызван после активации каждого игрового объекта из пула.
		/// </summary>
		/// <param name="callbackAfterEnable">Действие, которое выполняется после активации объекта.</param>
		public void SetAfterEnableCallback(Action<GameObject> callbackAfterEnable)
		{
			_callbackAfterEnable = callbackAfterEnable;
			foreach (var poolKey in _pools.Keys)
				_pools[poolKey].SetAfterEnableCallback(callbackAfterEnable);
		}

		/// <summary>
		/// Уничтожает все активные объекты во всех пулах.
		/// </summary>
		/// <param name="callbackBeforeKill">Действие, выполняемое перед уничтожением каждого объекта.</param>
		public void KillAll(Action<GameObject> callbackBeforeKill = null)
		{
			foreach (var pool in _pools.Values)
				pool.KillAll(callbackBeforeKill);
		}

		/// <summary>
		/// Устанавливает коллбэк, который будет вызван перед деактивацией каждого игрового объекта в пуле.
		/// </summary>
		/// <param name="callbackBeforeDisable">Действие, которое выполняется перед деактивацией объекта.</param>
		public void SetBeforeDisableCallback(Action<GameObject> callbackBeforeDisable)
		{
			_callbackBeforeDisable = callbackBeforeDisable;
			foreach (var poolKey in _pools.Keys)
				_pools[poolKey].SetBeforeDisableCallback(callbackBeforeDisable);
		}

		/// <summary>
		/// Создаёт и возвращает новый игровой объект из пула, соответствующего указанному ключу.
		/// </summary>
		/// <param name="key">Ключ, определяющий тип объекта в пуле.</param>
		/// <param name="additionalContext">Дополнительные параметры для передачи в конструктор объекта.</param>
		/// <returns>Созданный игровой объект.</returns>
		public GameObject Instantiate(TKey key, params object[] additionalContext)
		{
			var pool = GetOrCreatePoolObject(key);
			return pool.Instantiate(additionalContext);
		}

		/// <summary>
		/// Создаёт и возвращает новый объект типа <typeparamref name="TComponent"/> из пула,
		/// соответствующего указанному ключу.
		/// </summary>
		/// <typeparam name="TComponent">Тип компонента, который должен быть привязан к игровому объекту.</typeparam>
		/// <param name="key">Ключ, определяющий тип объекта в пуле.</param>
		/// <param name="additionalContext">Дополнительные параметры для передачи в конструктор объекта.</param>
		/// <returns>Созданный компонент типа <typeparamref name="TComponent"/>.</returns>
		public TComponent Instantiate<TComponent>(TKey key, params object[] additionalContext)
			where TComponent : IPooledObject
			=> Instantiate(key, additionalContext).GetComponent<TComponent>();
	}
}