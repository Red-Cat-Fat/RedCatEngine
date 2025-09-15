using System;
using JetBrains.Annotations;
using RedCatEngine.Pools.Containers.KeyContainers.Rules;
using RedCatEngine.Pools.Pools;

namespace RedCatEngine.Pools.Containers.KeyContainers
{
	public interface IKeyPoolComponentsContainer<in TKey, out TComponent> : IKeyPoolContainer<TKey>
		where TKey : IKeyRuleGameObjectSelector where TComponent : IPooledObject
	{
		/// <summary>
		/// Создаёт объект и возвращает экземпляр компонента <typeparamref name="TComponent"/> с этого объекта, который соответствующий указанному ключу.
		/// </summary>
		/// <param name="key">Ключ, определяющий пул для создаваемого объекта.</param>
		/// <param name="context">Дополнительные параметры для передачи в конструктор объекта (если требуется).</param>
		/// <returns>Созданный экземпляр компонента <typeparamref name="TComponent"/>.</returns>
		TComponent InstantiateComponent(TKey key, params object[] context);

		/// <summary>
		/// Уничтожает все активные объекты в пуле, вызывая указанный коллбэк перед уничтожением каждого компонента.
		/// </summary>
		/// <param name="callbackBeforeKill">Действие, которое будет выполнено перед уничтожением каждого компонента.</param>
		void KillAll([CanBeNull] Action<TComponent> callbackBeforeKill);
	}
}