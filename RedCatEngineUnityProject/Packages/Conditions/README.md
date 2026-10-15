**English** | [Русский](README.ru.md)

# Red Cat Engine: Conditions

Data-driven conditions: a check is assembled in the Inspector from ready blocks (and / or / not, value comparison, your own service checks) and evaluated at runtime.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.conditions` |
| Version | 1.1.1 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `Conditions` |
| Namespace | `RedCatEngine.Conditions` |

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Conditions
```

## Dependencies

| Package | Version | Used for |
|---|---|---|
| [Configs](../Configs/README.md) | 1.1.1 | `ConditionConfig` |
| [Dependency Injection](../DependencyInjection/README.md) | 1.2.1 | conditions read services from the container |
| [Values](../Values/README.md) | 1.1.1 | `CheckValueCondition`, `ConditionValue` |
| [Serialize Reference Editor](https://github.com/elmortem/serializereferenceeditor) | git | the `[SR]` type picker — add it to the project manually |

Values additionally requires [Common services](../UniversalServices/README.md) and [Pools](../Pools/README.md).

## Concepts

- **`ICondition`** — `bool Check(IGetterApplicationContainer container)`.
- **`IConditionCheckerService`** — checks a condition against the application container, so game code does not pass the container around.
- **`ConditionConfig`** — an abstract base for a condition stored as an asset; `ConditionConfigLink` references such an asset from another condition.
- **Bridge to Values** — `CheckValueCondition` turns an `IBoolValue` into a condition, `ConditionValue` turns a condition into an `IBoolValue`.

Built-in conditions:

| Picker name | Type | Result |
|---|---|---|
| Force Condition | `ForceCondition` | a fixed `true` / `false` |
| Logic/And | `AndCondition` | all nested conditions are true |
| Logic/Or | `OrCondition` | any nested condition is true |
| Logic/Not | `NotCondition` | inverts the nested condition |
| Logic Value Condition | `CheckValueCondition` | the result of an `IBoolValue` |
| Links/Condition Config | `ConditionConfigLink` | the result of a `ConditionConfig` asset |
| GameObject/Is Active GameObject in hierarchy | `IsActiveGameObject` | the target object is active |

## Usage

### A condition field

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

### Check

```csharp
using RedCatEngine.Conditions;

// the service checks conditions against the container: pass it as context
container.BindType<IConditionCheckerService, ConditionCheckerService>(container);

var conditions = container.GetSingle<IConditionCheckerService>();
if (conditions.Check(doorConfig.CanOpen))
	door.Open();
```

A condition can also be checked directly: `doorConfig.CanOpen.Check(container)`.

### Build a condition from code

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

### A custom condition

`BaseSingleServiceCondition<TService>` takes the service from the container and adds an **Invert** checkbox:

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

The condition throws if `IWallet` is not bound in the container.

### A condition as an asset

Create a condition asset from **Configs/Condition/Config container** and reference it with *Links/Condition Config*. A subclass can add its own menu item or fields:

```csharp
[CreateAssetMenu(menuName = "Configs/Condition")]
public class GameConditionConfig : ConditionConfig { }
```
