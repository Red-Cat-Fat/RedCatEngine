**English** | [Русский](README.ru.md)

# Red Cat Engine: Configs

Base classes for `ScriptableObject` configs with a stable numeric id, so a config can be referenced from save data and compared without holding the asset.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.configs` |
| Version | 1.1.1 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `Configs` |
| Namespace | `RedCatEngine.Configs` |

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Configs
```

## Dependencies

None.

## Concepts

- **`BaseConfig`** — `ScriptableObject` with a `uint Id`. The id is computed once from the asset GUID (FNV-1a hash) in `OnValidate` and then stored in the asset. Two configs are equal when they have the same type and the same `Id`.
- **`ConfigID<TConfig>`** — a serializable reference to a config by id. Converts implicitly from the config and compares with configs directly; `ConfigID<T>.Invalid` is the empty value.
- **`BaseAllConfigContainerConfig<TConfig>`** — an asset holding every config of a type, with lookup by id. In the editor `CollectLinks()` finds all assets of the type and fills the list.
- **`BaseSingleConfig<TConfig>`** — a config with a static `Instance`, loaded from `Resources/Configs/<TypeName>` (or `Resources/<TypeName>`). In the Editor a missing asset is created at `Assets/Resources/Configs/<TypeName>.asset`.

## Usage

### Declare a config

```csharp
using RedCatEngine.Configs;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Weapon", fileName = nameof(WeaponConfig))]
public class WeaponConfig : BaseConfig
{
	public int Damage;

	protected override void DoValidate()
	{
		// editor-only extra validation, called from OnValidate
	}
}
```

### Reference a config by id

```csharp
[Serializable]
public class PlayerSave
{
	public ConfigID<WeaponConfig> EquippedWeapon; // serializes as a number
}

save.EquippedWeapon = weaponConfig;              // implicit conversion

if (save.EquippedWeapon == weaponConfig) { /* same config */ }

if (allWeapons.TryFindById(save.EquippedWeapon, out var weapon)) { /* ... */ }
```

`TryFindById` and `AsConfigIdArray` are extension methods over `IEnumerable<TConfig>` (`BaseConfigExtensions`).

### A container with all configs of a type

```csharp
[CreateAssetMenu(menuName = "Configs/All Weapons", fileName = nameof(AllWeaponsConfig))]
public class AllWeaponsConfig : BaseAllConfigContainerConfig<WeaponConfig> { }
```

```csharp
if (allWeaponsConfig.TryGetById(weaponId, out WeaponConfig weapon)) { /* ... */ }
```

Call `CollectLinks()` from an editor script or a context menu to refresh the list.
