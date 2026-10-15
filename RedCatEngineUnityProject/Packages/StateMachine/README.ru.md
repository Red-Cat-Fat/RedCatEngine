[English](README.md) | **Русский**

# Red Cat Engine: State Machine

Типизированная машина состояний: к состояниям обращаются по типу, при входе им можно передать данные, а несколько состояний можно поставить в очередь и выполнить по порядку.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.state-machine` |
| Версия | 2.0.1 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `StateMachine` |
| Пространство имён | `RedCatEngine.StateMachine` |

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/StateMachine
```

## Зависимости

Нет. Чтобы создавать состояния через DI-контейнер, используйте `TypeBasedStateMachine` из пакета [Application Runner](../ApplicationRunner/README.ru.md).

## Основные понятия

- **`IState`** — `Enter()` / `Exit()`.
- **`IPayloadedState<TPayload>`** — `Enter(TPayload)` / `Exit()`; наследуйтесь от `BasePayloadState<TPayload>`, чтобы не писать шаблонный код.
- **`BaseTypedStateMachine<TBaseState>`** — базовый класс вашей машины. Состояния регистрируются защищёнными методами `AddState` и хранятся по типу; `ActiveState` — текущее состояние.
- **Очередь** — `AddToQueue` планирует состояния, `EnterNextFromQueue` входит в следующее, `ClearQueue` отбрасывает оставшиеся. Все вызовы возвращают машину, поэтому их можно объединять в цепочку.

Вход в состояние вызывает `Exit()` у предыдущего. Каждый переход пишется в лог Unity с именем машины.

## Использование

```csharp
using RedCatEngine.StateMachine.StateMachines;

public class MenuState : IState
{
	public void Enter() { /* показать меню */ }
	public void Exit() { /* скрыть меню */ }
}

public class LoadLevelState : BasePayloadState<string>
{
	public override void Enter(string levelName) { /* загрузить уровень */ }
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

### Очередь

```csharp
machine
	.AddToQueue<MenuState>()
	.AddToQueue<LoadLevelState, string>("Level_1")
	.EnterNextFromQueue(); // вход в MenuState

machine.EnterNextFromQueue(); // выход из MenuState, вход в LoadLevelState
machine.ClearQueue();
```

Типичный запуск игры кладёт в очередь все состояния инициализации, и каждое состояние по завершении вызывает `EnterNextFromQueue()`.

### Регистрация состояния под базовым типом

`BaseTypedStateMachine` предназначен для расширения: ваша машина решает, какие состояния в ней есть и какой класс реализует каждое из них. Состояние не обязательно регистрировать под его собственным классом — `AddState<TKey>(state)` сохраняет экземпляр под любым родительским классом или интерфейсом `TKey`, и остальная игра входит в него по этому ключу.

Так варианты для разных платформ и сборок не попадают в вызывающий код. Объявите шаг как базовый класс (или интерфейс) с несколькими реализациями и выберите одну при сборке машины — через `#if` или обычным `if`:

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

Вызывающий код знает только базовый тип и работает с любой сборкой:

```csharp
machine
	.AddToQueue<BaseInitializeNetworkState>()
	.EnterNextFromQueue();
```

Под одним ключом может быть зарегистрировано только одно состояние; повторная регистрация бросает `AlreadyContainStateException`. `TypeBasedStateMachine` из пакета [Application Runner](../ApplicationRunner/README.ru.md) делает то же самое без ручного создания экземпляров: `AddState<BaseInitializeNetworkState, InitializeSteamNetworkState>()` создаёт реализацию через DI-контейнер и регистрирует её под базовым типом.

## Ошибки

- `AlreadyContainStateException` — состояние такого типа уже зарегистрировано.
- `NotFoundStateException` — вход в незарегистрированное состояние.
- `EnterNextFromQueue()` при пустой очереди — не ошибка: ничего не происходит, текущее состояние остаётся активным.
