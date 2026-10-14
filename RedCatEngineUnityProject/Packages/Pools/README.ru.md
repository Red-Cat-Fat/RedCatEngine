[English](README.md) | **Русский**

# Red Cat Engine: Pools

Пулы объектов для префабов Unity: сериализуемые пулы, которые настраиваются в инспекторе, пулы с выбором префаба по ключу и события жизненного цикла объектов пула.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.pools` |
| Версия | 1.1.0 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `Pools` |
| Пространство имён | `RedCatEngine.Pools` |

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Pools
```

## Зависимости

Нет. Чтобы создавать объекты пула через DI-контейнер, используйте `DiPoolInstanceCreatorFactory` из пакета [Common services](../UniversalServices/README.ru.md).

## Основные понятия

- **`IPooledObject`** — то, что хранит пул: `Enable()`, `Disable()`, `Reset()`, `TeleportTo()` и `DeadEvent`. Объект возвращается в пул, когда вызывает `DeadEvent`; встроенные реализации делают это в `Disable()`.
- **`BasePooledObject`** — базовый `MonoBehaviour` с методами `DoEnable` / `DoDisable` / `DoReset` и методами для телепортации.
- **`CollectorComponentsPoolObject`** — автоматически добавляется на экземпляр, у префаба которого нет компонента `IPooledObject`. Передаёт события пула компонентам экземпляра, реализующим `IPooledEnable`, `IPooledDisable`, `IPooledReset`, `IPooledTeleportedLogic` или `IPooledCustomTeleported`.
- **Контейнеры** — `PoolObjectsContainer` / `PoolComponentsContainer<T>` для одного префаба, `KeyPoolContainer<TKey>` / `KeyPoolComponentsContainer<TKey, T>` — отдельный пул на каждый ключ.
- **Сериализуемые обёртки** — `UnityComponentPoolSerializable<T>` и `UnityKeyComponentsPoolSerializable<TKey, T>` — поля с `[Serializable]`, которые создают контейнер при первом обращении.

## Использование

### Объект пула

```csharp
using RedCatEngine.Pools.Pools;
using UnityEngine;

public class Bullet : BasePooledObject
{
	protected override void DoEnable() { /* взят из пула */ }
	protected override void DoReset() { /* вернуть начальное состояние */ }
	protected override void DoDisable() { /* возвращается в пул */ }

	private void OnCollisionEnter(Collision other)
		=> Disable(); // возвращает пулю в пул
}
```

### Пул, настроенный в инспекторе

```csharp
using RedCatEngine.Pools.Serializable;
using UnityEngine;

public class Gun : MonoBehaviour
{
	// префаб и родительский трансформ назначаются в инспекторе
	[SerializeField] private UnityComponentPoolSerializable<Bullet> _bullets;
	[SerializeField] private Transform _muzzle;

	public void Fire()
	{
		Bullet bullet = _bullets.InstantiateComponent(_muzzle.position);
	}

	private void OnDestroy()
		=> _bullets.KillAll();
}
```

Пул можно перебирать (`foreach (var bullet in _bullets)`), обращаться по индексу и узнавать `Length` — число активных объектов. `SetAfterEnableCallback` / `SetBeforeDisableCallback` подписывают на каждый объект, взятый из пула или возвращённый в него.

### Пул, созданный из кода

```csharp
using RedCatEngine.Pools.Containers.Creators.InstanceCreator;
using RedCatEngine.Pools.Containers.SimpleContainers;

var pool = new PoolComponentsContainer<Bullet>(
	new SimpleUnityInstanceCreator(bulletPrefab, parentTransform));

Bullet bullet = pool.InstantiateComponent();
```

### Пул по ключу

Ключ определяет, какой префаб использовать:

```csharp
using RedCatEngine.Pools.Containers.KeyContainers.Rules;

[CreateAssetMenu(menuName = "Configs/Enemy")]
public class EnemyConfig : ScriptableObject, IKeyRuleGameObjectSelector
{
	[SerializeField] private GameObject _prefab;

	public GameObject GetGameObjectForPool() => _prefab;
}
```

```csharp
[SerializeField] private UnityKeyComponentsPoolSerializable<EnemyConfig, Enemy> _enemies;

Enemy enemy = _enemies.InstantiateComponent(orcConfig);
```

### Поведение, собранное из компонентов

Префабу не обязательно иметь собственный `IPooledObject`. Оставьте префаб без него, реализуйте в компонентах только нужные методы — и пул сам добавит `CollectorComponentsPoolObject` на каждый созданный экземпляр:

```csharp
using RedCatEngine.Pools.Pools.Interfaces;

public class TrailCleaner : MonoBehaviour, IPooledReset
{
	[SerializeField] private TrailRenderer _trail;

	public void Reset() => _trail.Clear();
}
```

```csharp
[SerializeField] private UnityComponentPoolSerializable<CollectorComponentsPoolObject> _effects;

GameObject effect = _effects.Instantiate(position);
```

Не добавляйте `CollectorComponentsPoolObject` на префаб вручную: список компонентов собирается только тогда, когда его добавляет пул.
