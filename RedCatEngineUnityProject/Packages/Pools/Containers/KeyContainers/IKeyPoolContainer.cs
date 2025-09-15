using RedCatEngine.Pools.Containers.KeyContainers.Rules;
using RedCatEngine.Pools.Pools;
using UnityEngine;

namespace RedCatEngine.Pools.Containers.KeyContainers
{
	/// <summary>
	/// Интерфейс, предоставляющий функциональность пула игровых объектов, организованного по ключам.
	/// Позволяет создавать экземпляры игровых объектов на основе заданного ключа.
	/// </summary>
	/// <typeparam name="TKey">Тип ключа, используемого для группировки объектов в пуле.
	///     Должен реализовывать <see cref="IKeyRuleGameObjectSelector"/>.</typeparam>
	public interface IKeyPoolContainer<in TKey> : IPoolContainer where TKey : IKeyRuleGameObjectSelector
	{
		/// <summary>
		/// Создаёт и возвращает новый игровой объект из пула, соответствующего указанному ключу.
		/// </summary>
		/// <param name="key">Ключ, определяющий тип объекта в пуле.</param>
		/// <param name="additionalContext">Дополнительные параметры, передаваемые при создании объекта.</param>
		/// <returns>Созданный игровой объект.</returns>
		GameObject Instantiate(TKey key, params object[] additionalContext);

		/// <summary>
		/// Создаёт и возвращает новый игровой объект из пула, соответствующего указанному ключу.
		/// </summary>
		/// <param name="key">Ключ, определяющий тип объекта в пуле.</param>
		/// <param name="transformPosition">Позиция, куда создать объект</param>
		/// <param name="transformRotation">Поворот с которым создать объект</param>
		/// <param name="additionalContext">Дополнительные параметры, передаваемые при создании объекта.</param>
		/// <returns>Созданный игровой объект.</returns>
		GameObject Instantiate(TKey key, Vector3 transformPosition, Quaternion transformRotation, params object[] additionalContext);

		/// <summary>
		/// Создаёт новый игровой объект из пула, и возвращает компонент типа <typeparamref name="TComponent"/> из пула,
		/// соответствующего указанному ключу.
		/// </summary>
		/// <typeparam name="TComponent">Тип компонента, который должен быть привязан к игровому объекту.
		///     Должен реализовывать <see cref="IPooledObject"/>.</typeparam>
		/// <param name="key">Ключ, определяющий тип объекта в пуле.</param>
		/// <param name="additionalContext">Дополнительные параметры, передаваемые при создании объекта.</param>
		/// <returns>Созданный компонент типа <typeparamref name="TComponent"/>.</returns>
		TComponent Instantiate<TComponent>(TKey key, params object[] additionalContext)
			where TComponent : IPooledObject;
	}
}