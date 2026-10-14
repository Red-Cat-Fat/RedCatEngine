**English** | [Русский](README.ru.md)

# Red Cat Engine: Application runner

Building blocks for the game bootstrap: a state machine whose states are created by the DI container, ready-made start-up states (localization, cheats, player model loading) and a player model container with save triggers.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.aplication.runner` |
| Version | 1.1.0 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `ApplicationRunner` |
| Namespace | `RedCatEngine.ApplicationRunner` |

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/ApplicationRunner
```

## Dependencies

| Package | Version | Used for |
|---|---|---|
| [Dependency Injection](../DependencyInjection/README.md) | 1.2.0 | creating states, binding the player model |
| [State Machine](../StateMachine/README.md) | 2.0.0 | `BaseTypedStateMachine`, state interfaces |
| [Localization](https://github.com/elmortem/localization) | git | `LocalizeSystem` in the localization states — add it to the project manually |

Optional: `YandexInitializeLocalizationState` and `YandexModelLoader` are compiled only with the `YANDEX_GAME` scripting define and require the YandexGame plugin.

## Concepts

- **`TypeBasedStateMachine<TBaseState>`** — a [State Machine](../StateMachine/README.md) that creates states through `ICreator`. A state can be registered under an interface or a base class (`AddState<TTag, TImpl>(context)`, see [registering under a base type](../StateMachine/README.md#registering-a-state-under-a-base-type)), and a state that was not registered is created on its first `Enter`.
- **Start-up states** — each does its job and calls `EnterNextFromQueue()`:
  - `IInitializeLocalizationState` — `EditorLocalizationState` (fixed language) or `YandexInitializeLocalizationState` (language of the Yandex environment). Supported codes: `ru`, `en`, `tr`, `fr`, `de`; anything else falls back to English.
  - `ILoadPlayerModelState` — `LoadPlayerModelState<TModel, TLoader>` loads the player model and binds it in the container.
  - `IInitializeCheatState` — derive from `BaseInitializeCheatState` to spawn a cheat console prefab.
- **Player model** — `IPlayerModelContainer<TModel>` holds the model and saves it; `IModelLoader<TModel>` reads and writes it (`EditorModelLoader` uses `PlayerPrefs`, `YandexModelLoader` uses Yandex cloud saves); `IPlayerModelSaveTrigger.Save()` requests a save from anywhere.

## Usage

### The game state machine

```csharp
using System;
using RedCatEngine.ApplicationRunner.Infrastructure;
using RedCatEngine.ApplicationRunner.Infrastructure.States.InitializeLocalizationStates;
using RedCatEngine.ApplicationRunner.Infrastructure.States.LoadModelStates;
using RedCatEngine.ApplicationRunner.Meta.PlayerModels.ModelLoaders;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;
using RedCatEngine.StateMachine.StateMachines;

[Serializable]
public class PlayerModel
{
	public int Coins;
}

public class GameStateMachine : TypeBasedStateMachine<IState>
{
	[Inject]
	public GameStateMachine(ICreator creator) : base(creator, "Game")
	{
		// `this` and "en" are passed to the state constructor as context
		AddState<IInitializeLocalizationState, EditorLocalizationState>(this, "en");
		AddState<ILoadPlayerModelState, LoadPlayerModelState<PlayerModel, EditorModelLoader<PlayerModel>>>(this);
	}
}

public class GameLoopState : IState
{
	private readonly PlayerModel _model;

	[Inject]
	public GameLoopState(PlayerModel model) // bound by the load state
		=> _model = model;

	public void Enter() { /* start the game */ }
	public void Exit() { }
}
```

### Bootstrap

```csharp
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Specials.CreatedRule;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
	private UnityGameContainer _container;

	private void Awake()
	{
		_container = new UnityGameContainer(new UnityContainerModifyRule());

		var machine = _container.BindType<GameStateMachine>();
		machine
			.AddToQueue<IInitializeLocalizationState>()
			.AddToQueue<ILoadPlayerModelState>()
			.AddToQueue<GameLoopState>()   // not registered: created on enter
			.EnterNextFromQueue();
	}

	private void OnDestroy()
		=> _container.Dispose();
}
```

### Saving the player model

`LoadPlayerModelState` binds `IPlayerModelSaveTrigger`, `IPlayerModelContainer<PlayerModel>` and the loaded `PlayerModel`:

```csharp
public class Wallet
{
	private readonly PlayerModel _model;
	private readonly IPlayerModelSaveTrigger _saveTrigger;

	[Inject]
	public Wallet(PlayerModel model, IPlayerModelSaveTrigger saveTrigger)
	{
		_model = model;
		_saveTrigger = saveTrigger;
	}

	public void Add(int coins)
	{
		_model.Coins += coins;
		_saveTrigger.Save();
	}
}
```

To store the model elsewhere, implement `IModelLoader<TModel>` and use it as the second type argument of `LoadPlayerModelState`.

## Known limitations

- `EditorModelLoader.Load` and `YandexModelLoader.Load` invoke the callback with the loaded model and then again with a new empty model, so the saved model is replaced by an empty one. Until this is fixed, use your own `IModelLoader<TModel>` if saves must be restored.
- `BaseInitializeCheatState.Enter` instantiates the console prefab but does not call the abstract `AttachCheats()`.
