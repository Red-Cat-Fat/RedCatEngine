**English** | [Русский](README.ru.md)

# Red Cat Engine: State Machine

A typed state machine: states are addressed by their type, may receive a payload on enter and can be queued to run one after another.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.state-machine` |
| Version | 2.0.1 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `StateMachine` |
| Namespace | `RedCatEngine.StateMachine` |

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/StateMachine
```

## Dependencies

None. To create states through the DI container, use `TypeBasedStateMachine` from [Application Runner](../ApplicationRunner/README.md).

## Concepts

- **`IState`** — `Enter()` / `Exit()`.
- **`IPayloadedState<TPayload>`** — `Enter(TPayload)` / `Exit()`; derive from `BasePayloadState<TPayload>` to skip the boilerplate.
- **`BaseTypedStateMachine<TBaseState>`** — base class for your machine. States are registered with the protected `AddState` methods and stored by type; `ActiveState` is the current one.
- **Queue** — `AddToQueue` schedules states, `EnterNextFromQueue` enters the next one, `ClearQueue` drops the rest. All calls return the machine, so they can be chained.

Entering a state calls `Exit()` on the previous one. Every transition is written to the Unity log with the machine name.

## Usage

```csharp
using RedCatEngine.StateMachine.StateMachines;

public class MenuState : IState
{
	public void Enter() { /* show menu */ }
	public void Exit() { /* hide menu */ }
}

public class LoadLevelState : BasePayloadState<string>
{
	public override void Enter(string levelName) { /* load the level */ }
}

public class GameStateMachine : BaseTypedStateMachine<IExitableState>
{
	public GameStateMachine() : base("Game")
	{
		AddState(new MenuState());
		AddState(new LoadLevelState());
	}
}
```

```csharp
var machine = new GameStateMachine();

machine.Enter<MenuState>();
machine.Enter<LoadLevelState, string>("Level_1");
```

### Queue

```csharp
machine
	.AddToQueue<MenuState>()
	.AddToQueue<LoadLevelState, string>("Level_1")
	.EnterNextFromQueue(); // enters MenuState

machine.EnterNextFromQueue(); // exits MenuState, enters LoadLevelState
machine.ClearQueue();
```

A typical bootstrap puts all initialization states into the queue, and each state calls `EnterNextFromQueue()` when it is done.

### Registering a state under a base type

`BaseTypedStateMachine` is meant to be extended: your machine decides which states exist and which class implements each of them. A state does not have to be registered under its own class — `AddState<TKey>(state)` stores the instance under any base class or interface `TKey`, and the rest of the game enters it by that key.

This keeps platform- or build-specific variants out of the calling code. Declare the step as a base class (or an interface) with several implementations and pick one when the machine is built — with `#if` or with a regular `if`:

```csharp
public abstract class BaseInitializeNetworkState : IState
{
	public abstract void Enter();
	public virtual void Exit() { }
}

public class InitializeSteamNetworkState : BaseInitializeNetworkState { /* Steam */ }
public class InitializeMirrorNetworkState : BaseInitializeNetworkState { /* Mirror */ }

public class GameStateMachine : BaseTypedStateMachine<IExitableState>
{
	public GameStateMachine() : base("Game")
	{
#if STEAM_BUILD
		AddState<BaseInitializeNetworkState>(new InitializeSteamNetworkState());
#else
		AddState<BaseInitializeNetworkState>(new InitializeMirrorNetworkState());
#endif
	}
}
```

The calling code knows only the base type and works with either build:

```csharp
machine
	.AddToQueue<BaseInitializeNetworkState>()
	.EnterNextFromQueue();
```

Only one state can be registered under a key; registering a second one throws `AlreadyContainStateException`. `TypeBasedStateMachine` from [Application Runner](../ApplicationRunner/README.md) does the same without creating instances by hand: `AddState<BaseInitializeNetworkState, InitializeSteamNetworkState>()` creates the implementation through the DI container and registers it under the base type.

## Errors

- `AlreadyContainStateException` — a state of this type is already registered.
- `NotFoundStateException` — entering a state that was not registered.
- `EnterNextFromQueue()` on an empty queue is not an error: it does nothing and the current state stays active.
