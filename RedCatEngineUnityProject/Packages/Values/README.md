**English** | [Русский](README.ru.md)

# Red Cat Engine: Values

Data-driven values: a number or a flag in a config is not a constant but a small expression assembled in the Inspector — a constant, a global variable, a curve, an arithmetic or logic operation — and calculated at runtime.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.values` |
| Version | 1.1.0 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `Values` |
| Namespace | `RedCatEngine.Values` |

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Values
```

## Dependencies

| Package | Version | Used for |
|---|---|---|
| [Configs](../Configs/README.md) | 1.1.0 | value configs, `VariableConfig` |
| [Dependency Injection](../DependencyInjection/README.md) | 1.2.0 | values read services from the container |
| [Common services](../UniversalServices/README.md) | 2.0.0 | `IRedComponent`, `FloatDeltaChangeValue` |
| [Serialize Reference Editor](https://github.com/elmortem/serializereferenceeditor) | git | the `[SR]` type picker for `[SerializeReference]` fields — add it to the project manually |

Common services additionally requires [Pools](../Pools/README.md).

## Concepts

- **`IValue<T>`** — `T GetValue(IGetterApplicationContainer container)`. `IFloatValue`, `IBoolValue` and `IIntValue` are its typed versions; fields and methods are declared with them.
- **A value is an expression tree.** Most values hold other values in their fields: *Multiply* holds a list of float values, *Conditional Float* holds a bool value and two float values. The tree is assembled in the Inspector and evaluated top-down on every `GetValue` call.
- **Polymorphic fields** — a value is stored in a `[SerializeReference]` field; the `[SR]` attribute shows a picker with every available implementation, grouped by their `[SRName]`.
- **The container** — `GetValue` receives the application container, so a value can read game services (player health, wallet, variables).
- **`ValueCalculationService`** — calculates values against the container, so game code does not pass the container around; `Multiply(multiplier)` returns a service that scales every float result.
- **Variables** — `VariableConfig` is a named global float, `VariableContainer` holds current values and raises `ChangeVariableEvent`.

All built-in values are described in the [reference](#built-in-values) below.

## Usage

### A value field in a config

```csharp
using RedCatEngine.Configs;
using RedCatEngine.Values.Base.Interfaces;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Weapon")]
public class WeaponConfig : BaseConfig
{
	[SR]
	[SerializeReference]
	private IFloatValue _damage = new ConstantFloatValue(10f);

	public IFloatValue Damage => _damage;
}
```

### Calculate

```csharp
using RedCatEngine.Values.Services;

// the service reads values through the container: pass it as context
container.BindType<ValueCalculationService>(container);
container.BindType<VariableContainer>();

var values = container.GetSingle<ValueCalculationService>();
float damage = values.GetValue(weaponConfig.Damage);

// the same values with a x2 multiplier
float critical = values.Multiply(new ConstantFloatValue(2f)).GetValue(weaponConfig.Damage);
```

`GetValueOrDefault(value, fallback)` returns `fallback` when the field is empty.

### Global variables

```csharp
var variables = container.GetSingle<VariableContainer>();

variables.SetValue(difficultyVariable, 1.5f); // VariableConfig asset
variables.ChangeVariableEvent += (variable, change) => { /* change.OldValue, change.NewValue, change.Delta */ };
```

Every `VariableValue` that points to `difficultyVariable` now returns `1.5`. A variable that was never set returns `VariableConfig.DefaultValue`.

## Creating your own value

The built-in values cover constants, arithmetic and logic. Anything specific to your game — the player's health, the number of items, the time of day — is added as your own value, after which designers can combine it with the built-in ones without new code.

### 1. A value that reads a game service

```csharp
using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;

public interface IPlayerHealth
{
	float Current { get; }
	float Max { get; }
}

[Serializable]
[SRName("Player/Health percent")]
public class PlayerHealthPercentValue : IFloatValue
{
	public float GetValue(IGetterApplicationContainer container)
	{
		var health = container.GetSingle<IPlayerHealth>();
		return health.Max > 0f ? health.Current / health.Max : 0f;
	}
}
```

For the common case "take a service, return a number from it" there is a shorter base class, `BaseServiceGetterValue<TService, TResult>`:

```csharp
[Serializable]
[SRName("Player/Level")]
public class PlayerLevelValue : BaseServiceGetterValue<IPlayerProgress, float>, IFloatValue
{
	protected override float GetValueFromServices(IPlayerProgress progress)
		=> progress.Level;
}
```

### 2. A value with parameters

Parameters are serialized fields. A parameter that should itself be configurable is a nested value:

```csharp
using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

[Serializable]
[SRName("Operations/Power")]
public class PowerValue : IFloatValue
{
	[SR] [SerializeReference] private IFloatValue _base = ConstantFloatValue.One;
	[SR] [SerializeReference] private IFloatValue _exponent = ConstantFloatValue.One;

	public float GetValue(IGetterApplicationContainer container)
		=> Mathf.Pow(_base.GetValue(container), _exponent.GetValue(container));
}

[Serializable]
[SRName("Player/Has item")]
public class HasItemValue : IBoolValue
{
	[SerializeField] private ItemConfig _item;

	public bool GetValue(IGetterApplicationContainer container)
		=> container.GetSingle<IInventory>().Contains(_item);
}
```

### 3. Use it in game logic

A designer wants a "berserk" sword: double damage while the player has less than 30% health. No code changes are needed for this — the `Damage` field of the sword's `WeaponConfig` is assembled in the Inspector from the new value and built-in ones:

```text
Damage: Logic/Conditional Float
├── Check: Comparisons/Less
│   ├── Base:  Player/Health percent
│   └── Other: Common/Constant Float = 0.3
├── Value: Operations/Multiply
│   ├── Common/Constant Float = 20
│   └── Common/Constant Float = 2
└── Alternative: Common/Constant Float = 20
```

The game code only asks for the value:

```csharp
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.Values.Services;

public class Weapon
{
	private readonly WeaponConfig _config;
	private readonly ValueCalculationService _values;

	[Inject]
	public Weapon(WeaponConfig config, ValueCalculationService values)
	{
		_config = config;
		_values = values;
	}

	public void Hit(IDamageable target)
		=> target.TakeDamage(_values.GetValue(_config.Damage)); // 40 below 30% health, 20 otherwise
}

var sword = container.Create<Weapon>(swordConfig); // IPlayerHealth must be bound in the container
```

A value is evaluated anew on every call, so the result always reflects the current state of the game. Calculate it at the moment it is needed instead of caching it.

### Rules for your own values

- Mark the class `[Serializable]` and give it `[SRName("Group/Name")]` — the name groups it in the picker.
- Keep a parameterless constructor (implicit or explicit): the picker creates values through it.
- Initialize nested value fields (`= ConstantFloatValue.Zero`, `= ConstantBoolValue.False`), so a freshly picked value never meets `null`.
- Do not keep state in a value. One config asset — and therefore one value object — is shared by everything that uses it.
- Take services from the container passed to `GetValue`; the service must be bound there, otherwise `GetSingle` throws.
- For a bool check of a service with an *Invert* checkbox, `BaseSingleServiceCondition<TService>` from [Conditions](../Conditions/README.md) implements both `ICondition` and `IBoolValue`.

## Built-in values

The first column is the name in the `[SR]` picker.

### Constants

| Picker name | Type | Result |
|---|---|---|
| Common/Constant Float | `ConstantFloatValue` | the number from the field; in code `new ConstantFloatValue(5f)`, `ConstantFloatValue.Zero`, `ConstantFloatValue.One` |
| Common/Constant Int | `ConstantIntValue` | the integer from the field; `ConstantIntValue.Zero`, `ConstantIntValue.One` |
| Common/Constant Bool | `ConstantBoolValue` | the flag from the field; `ConstantBoolValue.True`, `ConstantBoolValue.False` |

### Variables and links

| Picker name | Type | Result |
|---|---|---|
| Common/Global Variable | `VariableValue` | the current value of a `VariableConfig` in `VariableContainer`, or its `DefaultValue` if it was never set. `VariableContainer` must be bound in the container |
| ConfigLink/ConfigLink Float | `FloatConfigLinkValue` | the value of a `FloatBaseValueConfig` asset |
| ConfigLink/ConfigLink Bool | `BoolConfigLinkValue` | the value of a `BoolBaseValueConfig` asset |

Value assets let several configs share one expression: create it once from **Configs/Values/Float** or **Configs/Values/Bool** and reference it with a *ConfigLink*. **Configs/Values/Variable** creates a `VariableConfig` with its `DefaultValue`.

### Operations

| Picker name | Type | Result |
|---|---|---|
| Operations/Sum | `AddValue` | the sum of all values in the list; `0` for an empty list |
| Operations/Subtract | `SubtractionValue` | `Base − Subtracted` |
| Operations/Multiply | `MultiplyValue` | the product of all values in the list; **`0` for an empty list** |
| Operations/Division | `DivisionValue` | `Base / Diver`; when the divisor is `0` it logs an error and returns `0` |
| Operations/Clamp | `Clamp` | the value limited to `[Min, Max]` (defaults: `0` and `1`) |

### Curves

Both take an `AnimationCurve` and an X value (any float value) and return the curve evaluated at X. They differ in what happens when X is outside the curve's time range:

| Picker name | Type | X outside the range |
|---|---|---|
| Curve/Clamp | `ClampCurveValue` | clamped to the range: the first or last key value |
| Curve/Repeat | `DuplicationCurveValue` | wrapped into the range: the curve repeats |

Example: damage growing with the player level along a designer-drawn curve — *Curve/Clamp* with X = *Player/Level*.

### Comparisons

| Picker name | Type | Result |
|---|---|---|
| Comparisons/More | `MoreValue` | `Base > Other` |
| Comparisons/Less | `LessValue` | `Base < Other` |
| Comparisons/Equals | `EqualsValue` | the absolute difference between `Base` and `Other` is less than `Tolerance`. Tolerance defaults to `0`, which makes the result always `false` — set it, for example to `0.001` |
| Comparisons/InInterval | `InIntervalValue` | `Min < Check < Max`; with *Is Equals* the edges count too (within *Tolerance*, default `Mathf.Epsilon`) |

### Logic

| Picker name | Type | Result |
|---|---|---|
| Logic/And | `AndValue` | `true` if both `Left` and `Right` are true |
| Logic/Or | `OrValue` | `true` if `Left` or `Right` is true |
| Logic/Not | `NotValue` | the opposite of `Value` |
| Logic/Conditional Float | `ConditionalFloatValue` | `Value` when `Check` is true, otherwise `Alternative` |
| Logic/Conditional Float (Multi) | `MultiConditionalFloatValue` | the `Value` of the first entry whose `Check` is true, otherwise `Default` — a chain of "if / else if / else" |

### Converters

| Picker name | Type | Result |
|---|---|---|
| Converters/Int to Float | `IntToFloatValue` | an int value as a float |
| Converters/Float to Int | `RoundFloatToIntValue` | a float value rounded to an int: *Nearest* (`Math.Round`, halves go to the even number: `2.5 → 2`) or *Ceiling* |

### From other packages

| Picker name | Type | Package | Result |
|---|---|---|---|
| Common/Condition value | `ConditionValue` | [Conditions](../Conditions/README.md) | the result of an `ICondition` |

### Base classes for your own values

| Type | Use it for |
|---|---|
| `BaseServiceGetterValue<TService, TResult>` | a value that takes one service and returns something from it |
| `BaseConfigLinkValue<T>` | a value that returns the value of a `BaseValueConfig<T>` asset |
| `BaseValueConfig<T>` | a new kind of value asset |
| `CurveValue` | a curve value with your own handling of X (`FilterXAxis`) |
| `IContextFloatValue<TContext>` | a float value that also needs runtime data, calculated with `ValueCalculationService.GetValue(value, context)`; there are no built-in implementations |

## Known limitations

- `CurveValue.MaxXValue` is marked `[SerializeReference]` instead of `[SerializeField]`, so it is not saved with the asset: a curve value set up in the Inspector has a maximum X of `0`. *Curve/Clamp* then clamps X to `[Min, 0]`, and *Curve/Repeat* can loop forever. Until this is fixed, create curve values in code (`new ClampCurveValue(curve, x)`) or through `CurveValueData.Make()`, which read the range from the curve keys.
