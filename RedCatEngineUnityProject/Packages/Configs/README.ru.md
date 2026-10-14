[English](README.md) | **Русский**

# Red Cat Engine: Configs

Базовые классы для конфигов на `ScriptableObject` со стабильным числовым идентификатором: на конфиг можно сослаться из сохранения и сравнивать конфиги, не держа ссылку на ассет.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.configs` |
| Версия | 1.1.0 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `Configs` |
| Пространство имён | `RedCatEngine.Configs` |

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Configs
```

## Зависимости

Нет.

## Основные понятия

- **`BaseConfig`** — `ScriptableObject` с полем `uint Id`. Идентификатор один раз вычисляется из GUID ассета (хеш FNV-1a) в `OnValidate` и сохраняется в ассете. Два конфига равны, если совпадают их тип и `Id`.
- **`ConfigID<TConfig>`** — сериализуемая ссылка на конфиг по идентификатору. Неявно создаётся из конфига и сравнивается с конфигами напрямую; `ConfigID<T>.Invalid` — пустое значение.
- **`BaseAllConfigContainerConfig<TConfig>`** — ассет со списком всех конфигов одного типа и поиском по идентификатору. В редакторе `CollectLinks()` находит все ассеты этого типа и заполняет список.
- **`BaseSingleConfig<TConfig>`** — конфиг со статическим `Instance`, который загружается из `Resources`.

## Использование

### Объявление конфига

```csharp
using RedCatEngine.Configs;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Weapon", fileName = nameof(WeaponConfig))]
public class WeaponConfig : BaseConfig
{
	public int Damage;

	protected override void DoValidate()
	{
		// дополнительная проверка в редакторе, вызывается из OnValidate
	}
}
```

### Ссылка на конфиг по идентификатору

```csharp
[Serializable]
public class PlayerSave
{
	public ConfigID<WeaponConfig> EquippedWeapon; // сериализуется как число
}

save.EquippedWeapon = weaponConfig;              // неявное преобразование

if (save.EquippedWeapon == weaponConfig) { /* тот же конфиг */ }

if (allWeapons.TryFindById(save.EquippedWeapon, out var weapon)) { /* ... */ }
```

`TryFindById` и `AsConfigIdArray` — методы расширения для `IEnumerable<TConfig>` (`BaseConfigExtensions`).

### Контейнер со всеми конфигами одного типа

```csharp
[CreateAssetMenu(menuName = "Configs/All Weapons", fileName = nameof(AllWeaponsConfig))]
public class AllWeaponsConfig : BaseAllConfigContainerConfig<WeaponConfig> { }
```

```csharp
if (allWeaponsConfig.TryGetById(weaponId, out WeaponConfig weapon)) { /* ... */ }
```

Чтобы обновить список, вызовите `CollectLinks()` из редакторного скрипта или контекстного меню.

## Известные ограничения

- `BaseSingleConfig<TConfig>.Instance` ищет ассет по буквальному имени `TConfig` (`Resources.Load<TConfig>("TConfig")`), а не по имени реального типа конфига. Пока это не исправлено, ассет нужно положить в папку `Resources` именно под таким именем.
