using UnityEngine;

namespace RedCatEngine.Pools.Containers.KeyContainers.Rules
{
	/// <summary>
	/// Интерфейс, определяющий правило выбора игрового объекта для пула на основе ключа.
	/// Реализации этого интерфейса используются для создания шаблонных объектов (префабов) в пуле,
	/// соответствующих конкретному типу или конфигурации.
	/// </summary>
	public interface IKeyRuleGameObjectSelector
	{
		/// <summary>
		/// Возвращает префаб (или игровой объект), который будет использоваться как основа
		/// при создании экземпляров в пуле.
		/// </summary>
		/// <returns>Префаб игрового объекта.</returns>
		GameObject GetGameObjectForPool();
	}
}