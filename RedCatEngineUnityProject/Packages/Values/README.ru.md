[English](README.md) | **Русский**

# Red Cat Engine: Values

Значения, управляемые данными: число или флаг в конфиге — не константа, а небольшое выражение, собранное в инспекторе (константа, глобальная переменная, кривая, арифметическая или логическая операция) и вычисляемое во время игры.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.values` |
| Версия | 1.1.0 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `Values` |
| Пространство имён | `RedCatEngine.Values` |

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Values
```

## Зависимости

| Пакет | Версия | Для чего |
|---|---|---|
| [Configs](../Configs/README.ru.md) | 1.1.0 | конфиги значений, `VariableConfig` |
| [Dependency Injection](../DependencyInjection/README.ru.md) | 1.2.0 | значения читают сервисы из контейнера |
| [Common services](../UniversalServices/README.ru.md) | 2.0.0 | `IRedComponent`, `FloatDeltaChangeValue` |
| [Serialize Reference Editor](https://github.com/elmortem/serializereferenceeditor) | git | выбор типа `[SR]` для полей с `[SerializeReference]` — добавляется в проект вручную |

Пакету Common services дополнительно нужен [Pools](../Pools/README.ru.md).

## Основные понятия

- **`IValue<T>`** — `T GetValue(IGetterApplicationContainer container)`. `IFloatValue`, `IBoolValue` и `IIntValue` — его типизированные версии; поля и методы объявляются через них.
- **Значение — это дерево выражения.** Большинство значений хранят в полях другие значения: *Multiply* — список чисел, *Conditional Float* — логическое значение и два числа. Дерево собирается в инспекторе и вычисляется сверху вниз при каждом вызове `GetValue`.
- **Полиморфные поля** — значение хранится в поле с `[SerializeReference]`; атрибут `[SR]` показывает список всех доступных реализаций, сгруппированных по их `[SRName]`.
- **Контейнер** — `GetValue` получает контейнер приложения, поэтому значение может читать игровые сервисы (здоровье игрока, кошелёк, переменные).
- **`ValueCalculationService`** — вычисляет значения относительно контейнера, чтобы игровой код не передавал контейнер повсюду; `Multiply(multiplier)` возвращает сервис, который умножает каждый результат типа float.
- **Переменные** — `VariableConfig` — именованное глобальное число, `VariableContainer` хранит текущие значения и вызывает `ChangeVariableEvent`.

Все встроенные значения описаны в [справочнике](#встроенные-значения) ниже.

## Использование

### Поле-значение в конфиге

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

### Вычисление

```csharp
using RedCatEngine.Values.Services;

// сервис читает значения через контейнер: передаём его в контексте
container.BindType<ValueCalculationService>(container);
container.BindType<VariableContainer>();

var values = container.GetSingle<ValueCalculationService>();
float damage = values.GetValue(weaponConfig.Damage);

// те же значения с множителем x2
float critical = values.Multiply(new ConstantFloatValue(2f)).GetValue(weaponConfig.Damage);
```

`GetValueOrDefault(value, fallback)` возвращает `fallback`, если поле пустое.

### Глобальные переменные

```csharp
var variables = container.GetSingle<VariableContainer>();

variables.SetValue(difficultyVariable, 1.5f); // ассет VariableConfig
variables.ChangeVariableEvent += (variable, change) => { /* change.OldValue, change.NewValue, change.Delta */ };
```

Теперь каждое `VariableValue`, ссылающееся на `difficultyVariable`, возвращает `1.5`. Переменная, значение которой не задавалось, возвращает `VariableConfig.DefaultValue`.

## Создание собственного значения

Встроенные значения покрывают константы, арифметику и логику. Всё, что специфично для вашей игры, — здоровье игрока, количество предметов, время суток — добавляется собственным значением, после чего геймдизайнеры комбинируют его со встроенными без нового кода.

### 1. Значение, читающее игровой сервис

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

Для частого случая «взять сервис и вернуть из него число» есть более короткий базовый класс `BaseServiceGetterValue<TService, TResult>`:

```csharp
[Serializable]
[SRName("Player/Level")]
public class PlayerLevelValue : BaseServiceGetterValue<IPlayerProgress, float>, IFloatValue
{
	protected override float GetValueFromServices(IPlayerProgress progress)
		=> progress.Level;
}
```

### 2. Значение с параметрами

Параметры — это сериализуемые поля. Параметр, который сам должен настраиваться, — вложенное значение:

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

### 3. Использование в игровой логике

Геймдизайнер хочет меч «берсерка»: двойной урон, пока у игрока меньше 30% здоровья. Для этого не нужно менять код — поле `Damage` в `WeaponConfig` меча собирается в инспекторе из нового значения и встроенных:

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

Игровой код только запрашивает значение:

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
		=> target.TakeDamage(_values.GetValue(_config.Damage)); // 40 при здоровье ниже 30%, иначе 20
}

var sword = container.Create<Weapon>(swordConfig); // IPlayerHealth должен быть зарегистрирован в контейнере
```

Значение вычисляется заново при каждом вызове, поэтому результат всегда соответствует текущему состоянию игры. Вычисляйте его в момент, когда оно нужно, а не кешируйте.

### Правила для собственных значений

- Пометьте класс `[Serializable]` и задайте `[SRName("Группа/Имя")]` — имя группирует его в списке выбора.
- Оставьте конструктор без параметров (неявный или явный): список выбора создаёт значения через него.
- Инициализируйте вложенные поля-значения (`= ConstantFloatValue.Zero`, `= ConstantBoolValue.False`), чтобы только что выбранное значение не встретило `null`.
- Не храните в значении состояние. Один ассет конфига — а значит, и один объект значения — общий для всех, кто его использует.
- Берите сервисы из контейнера, переданного в `GetValue`; сервис должен быть там зарегистрирован, иначе `GetSingle` бросит исключение.
- Для логической проверки сервиса с флажком *Invert* подойдёт `BaseSingleServiceCondition<TService>` из пакета [Conditions](../Conditions/README.ru.md): он реализует и `ICondition`, и `IBoolValue`.

## Встроенные значения

Первая колонка — имя в списке выбора `[SR]`.

### Константы

| Имя в списке выбора | Тип | Результат |
|---|---|---|
| Common/Constant Float | `ConstantFloatValue` | число из поля; в коде — `new ConstantFloatValue(5f)`, `ConstantFloatValue.Zero`, `ConstantFloatValue.One` |
| Common/Constant Int | `ConstantIntValue` | целое из поля; `ConstantIntValue.Zero`, `ConstantIntValue.One` |
| Common/Constant Bool | `ConstantBoolValue` | флаг из поля; `ConstantBoolValue.True`, `ConstantBoolValue.False` |

### Переменные и ссылки

| Имя в списке выбора | Тип | Результат |
|---|---|---|
| Common/Global Variable | `VariableValue` | текущее значение `VariableConfig` в `VariableContainer` или его `DefaultValue`, если значение не задавалось. `VariableContainer` должен быть зарегистрирован в контейнере |
| ConfigLink/ConfigLink Float | `FloatConfigLinkValue` | значение ассета `FloatBaseValueConfig` |
| ConfigLink/ConfigLink Bool | `BoolConfigLinkValue` | значение ассета `BoolBaseValueConfig` |

Ассеты значений позволяют нескольким конфигам использовать одно выражение: создайте его один раз из меню **Configs/Values/Float** или **Configs/Values/Bool** и ссылайтесь через *ConfigLink*. **Configs/Values/Variable** создаёт `VariableConfig` с его `DefaultValue`.

### Операции

| Имя в списке выбора | Тип | Результат |
|---|---|---|
| Operations/Sum | `AddValue` | сумма всех значений списка; `0` для пустого списка |
| Operations/Subtract | `SubtractionValue` | `Base − Subtracted` |
| Operations/Multiply | `MultiplyValue` | произведение всех значений списка; **`0` для пустого списка** |
| Operations/Division | `DivisionValue` | `Base / Diver`; при делителе `0` пишет ошибку в лог и возвращает `0` |
| Operations/Clamp | `Clamp` | значение, ограниченное `[Min, Max]` (по умолчанию `0` и `1`) |

### Кривые

Обе принимают `AnimationCurve` и значение X (любое числовое значение) и возвращают значение кривой в точке X. Различаются тем, что происходит, если X вне временного диапазона кривой:

| Имя в списке выбора | Тип | X вне диапазона |
|---|---|---|
| Curve/Clamp | `ClampCurveValue` | прижимается к диапазону: значение первого или последнего ключа |
| Curve/Repeat | `DuplicationCurveValue` | заворачивается в диапазон: кривая повторяется |

Пример: урон, растущий с уровнем игрока по кривой, нарисованной геймдизайнером, — *Curve/Clamp* с X = *Player/Level*.

### Сравнения

| Имя в списке выбора | Тип | Результат |
|---|---|---|
| Comparisons/More | `MoreValue` | `Base > Other` |
| Comparisons/Less | `LessValue` | `Base < Other` |
| Comparisons/Equals | `EqualsValue` | модуль разности `Base` и `Other` меньше `Tolerance`. По умолчанию *Tolerance* равен `0`, и результат всегда `false` — задайте его, например `0.001` |
| Comparisons/InInterval | `InIntervalValue` | `Min < Check < Max`; с *Is Equals* учитываются и границы (с точностью *Tolerance*, по умолчанию `Mathf.Epsilon`) |

### Логика

| Имя в списке выбора | Тип | Результат |
|---|---|---|
| Logic/And | `AndValue` | `true`, если истинны и `Left`, и `Right` |
| Logic/Or | `OrValue` | `true`, если истинно `Left` или `Right` |
| Logic/Not | `NotValue` | противоположное `Value` |
| Logic/Conditional Float | `ConditionalFloatValue` | `Value`, если `Check` истинно, иначе `Alternative` |
| Logic/Conditional Float (Multi) | `MultiConditionalFloatValue` | `Value` первой записи, у которой `Check` истинно, иначе `Default` — цепочка «если / иначе если / иначе» |

### Преобразования

| Имя в списке выбора | Тип | Результат |
|---|---|---|
| Converters/Int to Float | `IntToFloatValue` | целое значение как float |
| Converters/Float to Int | `RoundFloatToIntValue` | число, округлённое до целого: *Nearest* (`Math.Round`, половины округляются к чётному: `2.5 → 2`) или *Ceiling* |

### Из других пакетов

| Имя в списке выбора | Тип | Пакет | Результат |
|---|---|---|---|
| Common/Condition value | `ConditionValue` | [Conditions](../Conditions/README.ru.md) | результат `ICondition` |

### Базовые классы для собственных значений

| Тип | Для чего |
|---|---|
| `BaseServiceGetterValue<TService, TResult>` | значение, которое берёт один сервис и возвращает что-то из него |
| `BaseConfigLinkValue<T>` | значение, возвращающее значение ассета `BaseValueConfig<T>` |
| `BaseValueConfig<T>` | новый вид ассета-значения |
| `CurveValue` | значение-кривая со своей обработкой X (`FilterXAxis`) |
| `IContextFloatValue<TContext>` | числовое значение, которому нужны ещё и данные времени выполнения; вычисляется через `ValueCalculationService.GetValue(value, context)`; встроенных реализаций нет |

## Известные ограничения

- `CurveValue.MaxXValue` помечен `[SerializeReference]` вместо `[SerializeField]`, поэтому не сохраняется в ассете: у значения-кривой, настроенного в инспекторе, максимальный X равен `0`. Тогда *Curve/Clamp* прижимает X к `[Min, 0]`, а *Curve/Repeat* может зациклиться. Пока это не исправлено, создавайте значения-кривые в коде (`new ClampCurveValue(curve, x)`) или через `CurveValueData.Make()` — они берут диапазон из ключей кривой.
