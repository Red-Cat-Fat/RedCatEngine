[English](README.md) | **Русский**

# Red Cat Engine: Conditions

Условия, управляемые данными: проверка собирается в инспекторе из готовых блоков (и / или / не, сравнение значений, собственные проверки сервисов) и вычисляется во время игры.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.conditions` |
| Версия | 1.1.0 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `Conditions` |
| Пространство имён | `RedCatEngine.Conditions` |

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Conditions
```

## Зависимости

| Пакет | Версия | Для чего |
|---|---|---|
| [Configs](../Configs/README.ru.md) | 1.1.0 | `ConditionConfig` |
| [Dependency Injection](../DependencyInjection/README.ru.md) | 1.2.0 | условия читают сервисы из контейнера |
| [Values](../Values/README.ru.md) | 1.1.0 | `CheckValueCondition`, `ConditionValue` |
| [Serialize Reference Editor](https://github.com/elmortem/serializereferenceeditor) | git | выбор типа `[SR]` — добавляется в проект вручную |

Пакету Values дополнительно нужны [Common services](../UniversalServices/README.ru.md) и [Pools](../Pools/README.ru.md).

## Основные понятия

- **`ICondition`** — `bool Check(IGetterApplicationContainer container)`.
- **`IConditionCheckerService`** — проверяет условие относительно контейнера приложения, чтобы игровой код не передавал контейнер повсюду.
- **`ConditionConfig`** — абстрактная база для условия, хранимого как ассет; `ConditionConfigLink` ссылается на такой ассет из другого условия.
- **Связь с Values** — `CheckValueCondition` превращает `IBoolValue` в условие, `ConditionValue` превращает условие в `IBoolValue`.

Встроенные условия:

| Имя в списке выбора | Тип | Результат |
|---|---|---|
| Force Condition | `ForceCondition` | фиксированное `true` / `false` |
| Logic/And | `AndCondition` | все вложенные условия истинны |
| Logic/Or | `OrCondition` | хотя бы одно вложенное условие истинно |
| Logic/Not | `NotCondition` | инвертирует вложенное условие |
| Logic Value Condition | `CheckValueCondition` | результат `IBoolValue` |
| Links/Condition Config | `ConditionConfigLink` | результат ассета `ConditionConfig` |
| GameObject/Is Active GameObject in hierarchy | `IsActiveGameObject` | целевой объект активен |

## Использование

### Поле-условие

```csharp
using RedCatEngine.Conditions.Base;
using RedCatEngine.Conditions.Variants;
using RedCatEngine.Configs;
using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Door")]
public class DoorConfig : BaseConfig
{
	[SR]
	[SerializeReference]
	private ICondition _canOpen = ForceCondition.True;

	public ICondition CanOpen => _canOpen;
}
```

### Проверка

```csharp
using RedCatEngine.Conditions;

// сервис проверяет условия относительно контейнера: передаём его в контексте
container.BindType<IConditionCheckerService, ConditionCheckerService>(container);

var conditions = container.GetSingle<IConditionCheckerService>();
if (conditions.Check(doorConfig.CanOpen))
	door.Open();
```

Условие можно проверить и напрямую: `doorConfig.CanOpen.Check(container)`.

### Сборка условия из кода

```csharp
using RedCatEngine.Conditions.Variants.Logic;

var condition = new AndCondition
{
	Conditions = new ICondition[]
	{
		hasKeyCondition,
		new NotCondition { Condition = isLockedCondition },
	},
};
```

### Собственное условие

`BaseSingleServiceCondition<TService>` берёт сервис из контейнера и добавляет флажок **Invert**:

```csharp
using System;
using RedCatEngine.Conditions.Base;
using SerializeReferenceEditor;
using UnityEngine;

[Serializable]
[SRName("Player/Has coins")]
public class HasCoinsCondition : BaseSingleServiceCondition<IWallet>
{
	[SerializeField] private int _amount;

	protected override bool DoCheck(IWallet wallet)
		=> wallet.Coins >= _amount;
}
```

Условие бросает исключение, если `IWallet` не зарегистрирован в контейнере.

### Условие как ассет

`ConditionConfig` — абстрактный класс, поэтому один раз объявите конкретный тип и создавайте ассеты от него:

```csharp
[CreateAssetMenu(menuName = "Configs/Condition")]
public class GameConditionConfig : ConditionConfig { }
```
