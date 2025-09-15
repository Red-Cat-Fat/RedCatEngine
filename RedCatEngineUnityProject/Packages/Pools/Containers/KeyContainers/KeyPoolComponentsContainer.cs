using System;
using RedCatEngine.Pools.Containers.Creators.Factories;
using RedCatEngine.Pools.Containers.KeyContainers.Rules;
using RedCatEngine.Pools.Pools;
using UnityEngine;

namespace RedCatEngine.Pools.Containers.KeyContainers
{
	/// <summary>
	/// Контейнер пула компонентов, разделённый по ключам.
	/// Позволяет работать не просто с пулом GameObjects, а с пулом компонентов <typeparamref name="TComponent"/>, которые берутся с созданных объектов.
	/// </summary>
	/// <typeparam name="TKey">Тип ключа, используемого для группировки объектов в пуле. Должен реализовывать <see cref="IKeyRuleGameObjectSelector"/>, который будет возвращать объект для пула.</typeparam>
	/// <typeparam name="TComponent">Тип компонента, который создаётся и управляется внутри пула. Должен реализовывать <see cref="IPooledObject"/>.</typeparam>
	public class KeyPoolComponentsContainer<TKey, TComponent> : KeyPoolContainer<TKey>, IKeyPoolComponentsContainer<TKey, TComponent>
		where TKey : IKeyRuleGameObjectSelector
		where TComponent : IPooledObject
	{
		/// <summary>
		/// Инициализирует новый экземпляр <see cref="KeyPoolComponentsContainer{TKey, TComponent}"/>.
		/// </summary>
		/// <param name="creatorFactory">Фабрика, отвечающая за создание инстансов игровых объектов.</param>
		/// <param name="parentTransform">Родительский трансформ, к которому будут добавляться игровые объекты.</param>
		public KeyPoolComponentsContainer(IPoolInstanceCreatorFactory creatorFactory, Transform parentTransform)
			: base(creatorFactory, parentTransform)
		{
		}

		/// <summary>
		/// Создаёт объект и возвращает экземпляр компонента <typeparamref name="TComponent"/> с этого объекта, который соответствующий указанному ключу.
		/// </summary>
		/// <param name="key">Ключ, определяющий пул для создаваемого объекта.</param>
		/// <param name="context">Дополнительные параметры для передачи в конструктор объекта (если требуется).</param>
		/// <returns>Созданный экземпляр компонента <typeparamref name="TComponent"/>.</returns>
		public TComponent InstantiateComponent(TKey key, params object[] context)
			=> Instantiate<TComponent>(key, context);

		/// <summary>
		/// Уничтожает все активные объекты в пуле, вызывая указанный коллбэк перед уничтожением каждого компонента.
		/// </summary>
		/// <param name="callbackBeforeKill">Действие, которое будет выполнено перед уничтожением каждого компонента.</param>
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