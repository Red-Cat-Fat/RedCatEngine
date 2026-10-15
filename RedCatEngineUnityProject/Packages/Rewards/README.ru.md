[English](README.md) | **Русский**

# Red Cat Engine: Rewards

Награды, управляемые данными: то, что получает игрок, описывается в конфиге объектом награды и выдаётся одним сервисом.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.rewards` |
| Версия | 1.0.2 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `Rewards` |
| Пространство имён | `RedCatEngine.Rewards` |

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Rewards
```

## Зависимости

| Пакет | Версия | Для чего |
|---|---|---|
| [Conditions](../Conditions/README.ru.md) | 1.1.1 | `ConditionReward` |
| [Configs](../Configs/README.ru.md) | 1.1.1 | `RewardConfig` |
| [Dependency Injection](../DependencyInjection/README.ru.md) | 1.2.1 | награды читают сервисы из контейнера |
| [Values](../Values/README.ru.md) | 1.1.1 | поля-значения в наградах |
| [Serialize Reference Editor](https://github.com/elmortem/serializereferenceeditor) | git | выбор типа `[SR]` — добавляется в проект вручную |

Пакету Values дополнительно нужны [Common services](../UniversalServices/README.ru.md) и [Pools](../Pools/README.ru.md).

## Основные понятия

- **`IReward`** — `ApplyReward(IApplicationContainer)` выдаёт награду, `GetName(IApplicationContainer)` возвращает её отображаемое название. `IReward.Empty` — награда, которая ничего не делает.
- **`IRewardApplierService`** — выдаёт награду относительно контейнера приложения.
- **`RewardConfig`** — абстрактная база для награды, хранимой как ассет; `RewardConfigLink` ссылается на такой ассет из другой награды.

Встроенные награды:

| Тип | Поведение |
|---|---|
| `EmptyReward` | ничего |
| `CombinationReward` | выдаёт все награды из массива `Rewards` |
| `ConditionReward` | выдаёт `Reward`, если `Condition` истинно, иначе `AlternativeReward` |
| `RewardConfigLink` | выдаёт ассет `RewardConfig` |

## Использование

### Собственная награда

```csharp
using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Rewards.Base;
using SerializeReferenceEditor;
using UnityEngine;

[Serializable]
[SRName("Currency/Coins")]
public class CoinsReward : IReward
{
	[SerializeField] private int _amount;

	public string GetName(IApplicationContainer container)
		=> $"{_amount} coins";

	public void ApplyReward(IApplicationContainer container)
		=> container.GetSingle<IWallet>().Add(_amount);
}
```

### Поле-награда

```csharp
using RedCatEngine.Configs;
using RedCatEngine.Rewards.Base;
using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Chest")]
public class ChestConfig : BaseConfig
{
	[SR]
	[SerializeReference]
	private IReward _reward = IReward.Empty;

	public IReward Reward => _reward;
}
```

### Выдача

```csharp
using RedCatEngine.Rewards;

// сервис выдаёт награды относительно контейнера: передаём его в контексте
container.BindType<IRewardApplierService, RewardApplierService>(container);

var rewards = container.GetSingle<IRewardApplierService>();

label.text = rewards.GetName(chestConfig.Reward);
rewards.Apply(chestConfig.Reward);
```

### Награда как ассет

`RewardConfig` — абстрактный класс, поэтому один раз объявите конкретный тип и создавайте ассеты от него:

```csharp
[CreateAssetMenu(menuName = "Configs/Reward")]
public class GameRewardConfig : RewardConfig { }
```
