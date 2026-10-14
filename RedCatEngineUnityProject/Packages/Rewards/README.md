**English** | [Русский](README.ru.md)

# Red Cat Engine: Rewards

Data-driven rewards: what the player receives is described in a config as a reward object and applied by a single service.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.rewards` |
| Version | 1.0.1 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `Rewards` |
| Namespace | `RedCatEngine.Rewards` |

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Rewards
```

## Dependencies

| Package | Version | Used for |
|---|---|---|
| [Conditions](../Conditions/README.md) | 1.1.0 | `ConditionReward` |
| [Configs](../Configs/README.md) | 1.1.0 | `RewardConfig` |
| [Dependency Injection](../DependencyInjection/README.md) | 1.2.0 | rewards read services from the container |
| [Values](../Values/README.md) | 1.1.0 | value fields in rewards |
| [Serialize Reference Editor](https://github.com/elmortem/serializereferenceeditor) | git | the `[SR]` type picker — add it to the project manually |

Values additionally requires [Common services](../UniversalServices/README.md) and [Pools](../Pools/README.md).

## Concepts

- **`IReward`** — `ApplyReward(IApplicationContainer)` gives the reward, `GetName(IApplicationContainer)` returns its display name. `IReward.Empty` is a reward that does nothing.
- **`IRewardApplierService`** — applies a reward against the application container.
- **`RewardConfig`** — an abstract base for a reward stored as an asset; `RewardConfigLink` references such an asset from another reward.

Built-in rewards:

| Type | Behaviour |
|---|---|
| `EmptyReward` | nothing |
| `CombinationReward` | applies every reward from the `Rewards` array |
| `ConditionReward` | applies `Reward` when `Condition` is true, otherwise `AlternativeReward` |
| `RewardConfigLink` | applies a `RewardConfig` asset |

## Usage

### A custom reward

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

### A reward field

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

### Apply

```csharp
using RedCatEngine.Rewards;

// the service applies rewards against the container: pass it as context
container.BindType<IRewardApplierService, RewardApplierService>(container);

var rewards = container.GetSingle<IRewardApplierService>();

label.text = rewards.GetName(chestConfig.Reward);
rewards.Apply(chestConfig.Reward);
```

### A reward as an asset

`RewardConfig` is abstract, so declare a concrete type once and create assets from it:

```csharp
[CreateAssetMenu(menuName = "Configs/Reward")]
public class GameRewardConfig : RewardConfig { }
```
