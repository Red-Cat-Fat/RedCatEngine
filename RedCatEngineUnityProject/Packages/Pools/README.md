**English** | [Русский](README.ru.md)

# Red Cat Engine: Pools

Object pools for Unity prefabs: serializable pools that are configured in the Inspector, pools keyed by a config, and lifecycle hooks for pooled objects.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.pools` |
| Version | 1.1.0 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `Pools` |
| Namespace | `RedCatEngine.Pools` |

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Pools
```

## Dependencies

None. To create pooled objects through the DI container, use `DiPoolInstanceCreatorFactory` from [Common services](../UniversalServices/README.md).

## Concepts

- **`IPooledObject`** — what a pool stores: `Enable()`, `Disable()`, `Reset()`, `TeleportTo()` and `DeadEvent`. An object returns to its pool when it raises `DeadEvent`, which the built-in implementations do in `Disable()`.
- **`BasePooledObject`** — `MonoBehaviour` base class with `DoEnable` / `DoDisable` / `DoReset` / teleport hooks.
- **`CollectorComponentsPoolObject`** — added automatically to a pooled instance whose prefab has no `IPooledObject` component. It forwards pool events to the components of the instance that implement `IPooledEnable`, `IPooledDisable`, `IPooledReset`, `IPooledTeleportedLogic` or `IPooledCustomTeleported`.
- **Containers** — `PoolObjectsContainer` / `PoolComponentsContainer<T>` for one prefab, `KeyPoolContainer<TKey>` / `KeyPoolComponentsContainer<TKey, T>` for a pool per key.
- **Serializable wrappers** — `UnityComponentPoolSerializable<T>` and `UnityKeyComponentsPoolSerializable<TKey, T>` are `[Serializable]` fields that create the container lazily.

## Usage

### A pooled object

```csharp
using RedCatEngine.Pools.Pools;
using UnityEngine;

public class Bullet : BasePooledObject
{
	protected override void DoEnable() { /* taken from the pool */ }
	protected override void DoReset() { /* restore the initial state */ }
	protected override void DoDisable() { /* going back to the pool */ }

	private void OnCollisionEnter(Collision other)
		=> Disable(); // returns the bullet to the pool
}
```

### A pool configured in the Inspector

```csharp
using RedCatEngine.Pools.Serializable;
using UnityEngine;

public class Gun : MonoBehaviour
{
	// assign the prefab and the parent transform in the Inspector
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

The pool can be enumerated (`foreach (var bullet in _bullets)`), indexed, and reports `Length` — the number of currently active objects. `SetAfterEnableCallback` / `SetBeforeDisableCallback` subscribe to every object taken from or returned to the pool.

### A pool created from code

```csharp
using RedCatEngine.Pools.Containers.Creators.InstanceCreator;
using RedCatEngine.Pools.Containers.SimpleContainers;

var pool = new PoolComponentsContainer<Bullet>(
	new SimpleUnityInstanceCreator(bulletPrefab, parentTransform));

Bullet bullet = pool.InstantiateComponent();
```

### A pool per key

The key decides which prefab to use:

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

### Composing behaviour from components

A prefab does not need its own `IPooledObject`. Leave it without one, implement only the hooks each component needs, and the pool adds `CollectorComponentsPoolObject` to every instance it creates:

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

Do not add `CollectorComponentsPoolObject` to the prefab by hand: the component list is collected only when the pool adds it.
