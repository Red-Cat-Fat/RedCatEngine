**English** | [Русский](README.ru.md)

# Red Cat Engine: Common services

A set of general-purpose services and helper types used by games and by other Red Cat Engine packages: logging, time, update loop, delayed callbacks, random, save/load, tags, sound, storages and extensions.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.services` |
| Version | 2.0.1 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `CommonServices` |
| Namespace | `RedCatEngine.CommonServices` |

The folder is called `UniversalServices`, while the assembly and the namespace are `CommonServices`.

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/UniversalServices
```

## Dependencies

| Package | Version | Used for |
|---|---|---|
| [Configs](../Configs/README.md) | 1.1.1 | `TagConfig`, `BaseItemConfig`, `TimeSettingsConfig` |
| [Dependency Injection](../DependencyInjection/README.md) | 1.2.1 | `[Inject]` constructors, `MonoConstruct` components |
| [Pools](../Pools/README.md) | 1.1.0 | `DiPoolInstanceCreatorFactory` |

Optional: `DoTweenExtensions` is compiled only when the `DOTWEEN` scripting define is set (DOTween installed).

## What is inside

| Area | Main types |
|---|---|
| Logs | `ILogService`, `LogHub`, `LogRouter`, `LogLevelFilter`, `FileLogSink`, `UnityConsoleSink` |
| Time | `ITimeService`, `IPausedTimeService`, `UnityTimeService` |
| Update loop | `IUpdatableService`, `UpdatableService`, `UnityUpdater`, `MonoConstructUpdatable` |
| Callbacks | `IDisposableCallbackService`, `DisposableCallback`, `DisposableTimerCallback` |
| Random | `IRandomService`, `UnityRandomService` |
| Save | `ISaved`, `ISaveApplierContainer`, `ISaveLoadReaderService`, `PlayerSaveService` |
| Tags | `TagConfig`, `ITagStorage`, `TagService` |
| Sound | `ISoundService`, `SoundService`, `SoundSourceContainer`, `SoundSfxContainer` |
| Cursor | `IMouseLockerService` |
| Storages | `BaseStorage<T>`, `BaseCountedStorage<T>`, `TypedStorage`, `RedComponentContainer`, `Observable<T>` |
| Special types | `ChangeValue<T>`, `DeltaChangeValue<T>`, `ChangeTrackedValue<T>`, `UnityTag`, `ComponentCollector<T>` |
| Pools + DI | `DiPoolInstanceCreatorFactory`, `DiInstanceCreator` |
| Extensions | arrays, strings, vectors, `AnimationCurve`, `LayerMask`, gizmos |

## Usage

### Logs

A log service writes through a router to one or more sinks; a filter decides which levels pass, globally and per tag.

```csharp
using RedCatEngine.CommonServices.Services.Logs;
using RedCatEngine.CommonServices.Services.Logs.Catching;
using RedCatEngine.CommonServices.Services.Logs.Filtering;
using RedCatEngine.CommonServices.Services.Logs.Sinks;

var filter = new LogLevelFilter(new PlayerPrefsLogFilterStorage(persistOverrides: true), LogLevel.Info);
var fileSink = new FileLogSink("Logs", "game");
var router = new LogRouter(filter, new CompositeLogSink(new UnityConsoleSink(), fileSink));
ILogService rootLog = new RoutedLogService(router);

LogHub.Initialize(rootLog, filter, router, fileSink, new UnhandledLogBuffer());
container.BindAsSingle<ILogService>(rootLog);
```

```csharp
public class EnemySpawner
{
	private readonly ILogService _log;

	[Inject]
	public EnemySpawner(ILogService log)
		=> _log = log.CreateTag<EnemySpawner>(); // every message is tagged

	public void Spawn(string id)
	{
		_log.Info("Spawn enemy {0}", id);
		_log.LogWarning("Spawn point is busy");
	}
}
```

`Verbose` / `Info` calls are compiled only in the Editor, in development builds or with the `CHEAT_ENABLED` define; warnings and errors are always written. Code that has no container can use `LogHub.CreateTag<T>()`.

#### Tag hierarchy

`CreateTag` returns a new log service whose tag is appended to the tag of the service it was created from, separated by `:`. Each system creates a tag from the log it received, so the tags repeat the structure of the game:

```csharp
ILogService gameLog = rootLog.CreateTag("Game");                // Game
ILogService battleLog = gameLog.CreateTag("Battle");            // Game:Battle
ILogService spawnerLog = battleLog.CreateTag<EnemySpawner>();   // Game:Battle:EnemySpawner
ILogService wavesLog = battleLog.CreateTag("Waves");            // Game:Battle:Waves
ILogService uiLog = gameLog.CreateTag("UI");                    // Game:UI
ILogService orcLog = spawnerLog.CreateTag(orc.gameObject);      // Game:Battle:EnemySpawner:Orc_01

spawnerLog.Info("Spawn enemy {0}", "Orc");
wavesLog.Info("Wave {0} started", 3);
uiLog.Info("Open window {0}", "Shop");
```

Unity console:

```text
[Game:Battle:EnemySpawner] Spawn enemy Orc
[Game:Battle:Waves] Wave 3 started
[Game:UI] Open window Shop
```

Log file (`FileLogSink`), with local time, level and thread id:

```text
[14:02:11.384] [INFO] [T1] [Game:Battle:EnemySpawner] Spawn enemy Orc
[14:02:11.390] [INFO] [T1] [Game:Battle:Waves] Wave 3 started
[14:02:12.007] [INFO] [T1] [Game:UI] Open window Shop
```

The tag also makes the output easy to filter: search the console or the file for `Game:Battle` to see only the battle.

The level filter works on the same hierarchy. A level set for a tag applies to every tag below it, and the most specific setting wins:

```csharp
filter.SetGlobalLevel(LogLevel.Warning);              // everything: warnings and errors only
filter.SetLevel("Game:Battle", LogLevel.Verbose);     // the whole battle: everything
filter.SetLevel("Game:Battle:Waves", LogLevel.Off);   // except waves: nothing
```

| Tag | Level in effect |
|---|---|
| `Game:UI` | Warning (global) |
| `Game:Battle:EnemySpawner` | Verbose (from `Game:Battle`) |
| `Game:Battle:EnemySpawner:Orc_01` | Verbose (from `Game:Battle`) |
| `Game:Battle:Waves` | Off |

`CreateTag(string)` and `CreateTag<T>()` register the tag in the filter, so it appears in the tag tree of the **Tools > 🐛 Log Filter** window, where levels can be changed while the game is running. `CreateTag(GameObject)` / `CreateInstanceTag(name)` are meant for per-object logs: they are filtered by their parent's settings but are not added to the tree. Overrides are saved by the `ILogFilterStorage` passed to `LogLevelFilter`. **Tools > Log Viewer** opens the log files.

### Time and update loop

`UnityTimeService` is a scene component that implements `ITimeService` (delta time, fixed delta time, time scale, total time) and `IPausedTimeService`:

```csharp
container.BindAsSingle<ITimeService>(unityTimeService);
container.BindAsSingle<IPausedTimeService>(unityTimeService);
```

Pause is tracked per source — `pausedTime.Pause(this)` / `pausedTime.UnPause(this)` — and time stays paused while at least one source holds it.

#### Update for classes that are not MonoBehaviours

In Unity only a `MonoBehaviour` receives `Update`, `FixedUpdate` and `LateUpdate`. Game logic, however, usually lives in plain C# classes created by the DI container: services, controllers, AI, timers. Without the update loop such a class would need a helper `MonoBehaviour` only to forward `Update` to it, or would have to be a `MonoBehaviour` itself.

`IUpdatableService` gives plain classes the same three callbacks:

| Interface | Method | Called from |
|---|---|---|
| `IUpdatable` | `OnUpdate()` | `Update` |
| `IFixedUpdatable` | `OnFixedUpdate()` | `FixedUpdate` |
| `ILateUpdatable` | `OnLateUpdate()` | `LateUpdate` |

One `UnityUpdater` component in the scene (it implements `IUpdateTrigger`) receives the Unity callbacks and passes them to `UpdatableService`, which calls every registered object. The scene has a single `MonoBehaviour` with `Update` instead of one per system.

Register the service once:

```csharp
using RedCatEngine.CommonServices.Services.UpdatableService;

container.BindAsSingle<IUpdateTrigger>(unityUpdater);
container.BindType<IUpdatableService, UpdatableService>("Game"); // "Game" is the tag used in error messages
```

A class subscribes when it starts and unsubscribes when it ends. `OnUpdate` has no parameters; take the frame time from `ITimeService`, so the class follows the game's pause and time scale:

```csharp
using System;
using RedCatEngine.CommonServices.Services.Times.TimeServices;
using RedCatEngine.CommonServices.Services.UpdatableService;
using RedCatEngine.DependencyInjection.Containers.Attributes;

public class EnemyAi : IUpdatable, IFixedUpdatable, IDisposable
{
	private readonly IUpdatableService _updates;
	private readonly ITimeService _time;
	private float _thinkCooldown;

	[Inject]
	public EnemyAi(IUpdatableService updates, ITimeService time)
	{
		_updates = updates;
		_time = time;

		_updates.AddUpdatable(this);
		_updates.AddFixedUpdatable(this);
	}

	public void OnUpdate()
	{
		_thinkCooldown -= _time.DeltaTime;
		if (_thinkCooldown > 0f)
			return;

		_thinkCooldown = 0.5f;
		ChooseTarget();
	}

	public void OnFixedUpdate()
		=> MoveToTarget(_time.FixedDeltaTime);

	public void Dispose()
	{
		_updates.RemoveUpdated(this);
		_updates.RemoveFixedUpdated(this);
	}

	private void ChooseTarget() { /* ... */ }
	private void MoveToTarget(float deltaTime) { /* ... */ }
}
```

The DI container disposes objects it created when the container is disposed, so `Dispose` is the place to unsubscribe. An object that is not unsubscribed keeps being updated and is never collected.

How `UpdatableService` behaves:

- objects are called in the order they were added; adding the same object twice has no effect;
- adding or removing during an update is applied after the current pass, so an object may safely remove itself in `OnUpdate`;
- if an object throws, the exception is logged through `LogHub` with the service tag and the object (for a component — the `GameObject` path), and then rethrown, so the remaining objects are not called in that frame;
- `Dispose()` unsubscribes the service from `IUpdateTrigger` and clears all lists.

Several services with different tags can be bound in different containers, for example one per level, and disposed together with that scope.

#### Components

A component created by the DI container can derive from `MonoConstructUpdatable` or `MonoConstructLateUpdatable`. It receives `IUpdatableService` and `ITimeService` through `[MonoInject]`, registers itself after initialization, unregisters on dispose and gets the frame time as a parameter:

```csharp
public class Rotator : MonoConstructUpdatable
{
	protected override void DoUpdate(float deltaTime)
		=> transform.Rotate(0f, 90f * deltaTime, 0f);

	protected override void DoSubscribeAfterInitialize() { /* instead of DoInitialize */ }
	protected override void DoUnsubscribeAfterDispose() { /* instead of DoDispose */ }
}
```

`TestedMonoConstructUpdatable` is a variant for debug-only components: it updates in the Editor and, in a build, logs an error once instead of updating.

### Delayed callbacks

`IDisposableCallbackService` creates callbacks that are run at most once and are controlled by the object returned to you — a **token**. Through the token you cancel the callback (`Dispose()`), run it early (`Invoke()`), or check whether it is still pending (`IsEnable`).

```csharp
using RedCatEngine.CommonServices.Services.DisposableCallbacks;

container.BindType<IDisposableCallbackService, DisposableCallbackService>(); // needs ITimeService
```

#### Kinds of callbacks

| Method | Token | Runs | On `Dispose()` before it ran |
|---|---|---|---|
| `MakeTimerCallback(time, callback)` | `DisposableTimerCallback` | after `time` seconds | cancelled, never runs |
| `MakeAlwaysInvokeTimerCallback(time, callback)` | `DisposableTimerCallback` | after `time` seconds | **runs immediately** |
| `MakeCallback(callback)` | `DisposableCallback` | when you call `Invoke()` | cancelled, never runs |
| `MakeAlwaysInvokeCallback(callback)` | `DisposableCallback` | when you call `Invoke()` | **runs immediately** |
| `MakeCallback<TResult>(callback)` | `DisposableCallback<TResult>` | when you call `Invoke(result)` | cancelled, never runs |

In every case the callback runs **at most once**: after `Invoke()` (or after the timer fires) further `Invoke()` and `Dispose()` calls do nothing.

Timers count the delta time of `ITimeService.UpdateEvent`. With `UnityTimeService` this is the scaled `Time.deltaTime`, so timers slow down together with the game, including during a pause. `TimeLeft` shows how many seconds remain.

#### Timers that can be cancelled

Use them for something that should happen later only if nothing changed in the meantime:

```csharp
// hide the hint in 3 seconds, unless the player closes it first
_hideHint = _callbacks.MakeTimerCallback(3f, () => _hint.Hide());

public void OnHintClosedByPlayer()
{
	_hideHint?.Dispose(); // the timer is cancelled, Hide() is not called
	_hint.Hide();
}
```

#### Callbacks that always run

The *always invoke* variants guarantee that the callback runs exactly once: when the timer fires or `Invoke()` is called — or, at the latest, when the token is disposed. Use them for cleanup that must not be lost, like a `finally` block:

```csharp
_pause.Pause(this);
// unpause after 2 seconds — or earlier, if the token is disposed
_resumeGame = _callbacks.MakeAlwaysInvokeTimerCallback(2f, () => _pause.UnPause(this));

public void Skip()
	=> _resumeGame.Dispose(); // UnPause runs now, not in 2 seconds
```

```csharp
// the loading screen is closed whether loading succeeds, fails or is abandoned
var closeLoading = _callbacks.MakeAlwaysInvokeCallback(() => _loadingScreen.Hide());
try
{
	await LoadLevelAsync();
}
finally
{
	closeLoading.Dispose();
}
```

Remember that disposing an *always invoke* token **runs** the callback. Do not use these variants for actions that must not happen on cancel, and make sure everything the callback touches still exists at that moment.

#### A one-time callback with a result

`MakeCallback<TResult>` turns a "reply once" contract into a token, for example for a dialog:

```csharp
public DisposableCallback<bool> AskConfirmation(Action<bool> onAnswer)
{
	_answer = _callbacks.MakeCallback(onAnswer);
	_dialog.Show();
	return _answer;
}

public void OnYes() => _answer.Invoke(true);  // the first answer wins
public void OnNo() => _answer.Invoke(false);  // later answers are ignored
```

The caller may dispose the returned token to stop waiting for the answer.

#### Keeping tokens

The token is the only way to control a callback, so store it in a field for as long as the callback may still run:

```csharp
public class Turret : IDisposable
{
	private readonly IDisposableCallbackService _callbacks;
	private DisposableTimerCallback _reload;

	[Inject]
	public Turret(IDisposableCallbackService callbacks)
		=> _callbacks = callbacks;

	public bool IsReloading => _reload is { IsEnable: true };

	public void Fire()
	{
		if (IsReloading)
			return;

		_reload?.Dispose();                                   // drop the previous token before replacing it
		_reload = _callbacks.MakeTimerCallback(1.5f, OnReloaded);
	}

	public void Dispose()
		=> _reload?.Dispose();                                // the turret is gone: cancel the reload

	private void OnReloaded() { /* ... */ }
}
```

Rules that keep tokens safe:

- **Dispose tokens together with their owner** — in `Dispose()` of a class or in `DoDispose()` / `OnDestroy()` of a component. Otherwise the timer can fire after the owner is destroyed and call into a destroyed object.
- **Dispose the old token before replacing it**, as in `Fire()` above, or the old timer keeps running.
- **Several tokens of one owner** can be kept in a `List<IDisposable>` and disposed in a loop.
- **Check `IsEnable`** to learn whether the callback is still pending; it becomes `false` after the callback ran or was cancelled.
- Disposing `DisposableCallbackService` disposes every token it created: ordinary callbacks are cancelled, *always invoke* callbacks run. A service bound in a child container (for example per level) therefore cleans up its callbacks together with the level.
- Callbacks that have run or were cancelled are released by the service, so many short-lived callbacks do not accumulate.

### Random

```csharp
IRandomService random = new UnityRandomService();
random.SetSeed(42);

int index = random.GetRange(0, 10);          // max is exclusive
var enemy = random.GetRandomElement(enemies);
var order = random.GetShuffle(cards);
float damage = random.ErrorRate(100f, 0.1f);
```

### Save

The save system collects the state of every service that must be saved into one save, writes it under a key and gives each service its own part back on load. Services do not know where and how the save is stored, and the storage does not know what the services contain.

#### How it works

```text
          Save()                                        Load()
            │                                             │
   PlayerSaveService ("Saves" key)               PlayerSaveService
            │                                             │
   ISaveLoadReaderService ── JSON ──► storage ── JSON ──► ISaveLoadReaderService
            │   (PlayerPrefs, file, cloud…)                │
   ISaveApplierContainer                           ISaveApplierContainer
     GenerateSaveWithName(key)                       Load(SaveData)
            │                                             │
   every ISaved: GetSerializedSaveData()          every ISaved: Load(its json)
```

The save is a `SaveData`: a name and an array of `ServiceSaveData` entries, one per service — the service's `SaveKey` and the string it returned. Each service serializes its own data; the reader then serializes the whole `SaveData` with `JsonUtility` and stores the resulting string.

#### The parts

| Type | Role | What you do with it |
|---|---|---|
| `ISaved` | A service whose state is saved: `SaveKey`, `GetSerializedSaveData()`, `Load(string)` | **Implement** in every service that has persistent state |
| `SaveSerializer<TData>` | `JsonUtility` helper for a service's data class; returns a new `TData` for an empty string | Use inside `ISaved` implementations |
| `ISaveApplierContainer` / `SaveApplierContainer` | Collects the `ISaved` services, builds `SaveData` from them and hands entries back on load | Bind once |
| `ISaveLoadReaderService` / `BaseSaveLoadReaderService` | Reads and writes `SaveData` JSON under a key; logs every operation | Bind one implementation; **derive** your own for another storage |
| `PlayerPrefSaveLoadReaderService` | Storage in `PlayerPrefs` | Ready to use |
| `PlayerSaveService` | Saves, loads and erases the save under the fixed key `"Saves"` | Call from game code |

#### 1. Make a service saveable

Keep the data in a separate `[Serializable]` class and reference configs by `ConfigID`, not by the asset:

```csharp
using System;
using RedCatEngine.CommonServices.Services.Save.Realisations;
using RedCatEngine.Configs;

[Serializable]
public class WalletData
{
	public int Coins;
	public ConfigID<SkinConfig> SelectedSkin;
}

public class WalletService : IWallet, ISaved
{
	private readonly SaveSerializer<WalletData> _serializer = new();
	private WalletData _data = new();

	public string SaveKey => "Wallet";   // unique and stable: renaming it loses old saves

	public int Coins => _data.Coins;

	public void Add(int coins) => _data.Coins += coins;

	public string GetSerializedSaveData()
		=> _serializer.SaveDataFrom(_data);

	public void Load(string json)
		=> _data = _serializer.LoadDataFrom(json); // json is null when the save has no entry for this key
}
```

`Load` receives `null` for a service that has no entry in the save — on the first launch or when the service was added in a newer version of the game. `SaveSerializer.LoadDataFrom` returns a new data object in that case, so the service simply starts with defaults.

#### 2. Register the services

`SaveApplierContainer` receives the `ISaved` services through the DI container: it is an array waiter for `ISaved`, so every instance bound **after it** with `BindAsArray<ISaved>` is attached to it.

```csharp
using RedCatEngine.CommonServices.Services.Save;
using RedCatEngine.CommonServices.Services.Save.Applier;
using RedCatEngine.CommonServices.Services.Save.Realisations;
using RedCatEngine.CommonServices.Services.Save.SaveReader;

// 1. the collector — before any ISaved service
container.BindType<ISaveApplierContainer, SaveApplierContainer>();

// 2. the storage and the facade
container.BindType<ISaveLoadReaderService, PlayerPrefSaveLoadReaderService>(); // needs ILogService
var saves = container.BindType<PlayerSaveService>();

// 3. services: bind as usual, then register each one as ISaved
var wallet = container.BindType<IWallet, WalletService>();
container.BindAsArray<ISaved>((ISaved)wallet);

var tags = container.BindType<ITagService, TagService>();
container.BindAsArray<ISaved>(tags);
```

Two details matter here:

- the type argument of `BindAsArray` must be exactly `ISaved` — that is the key the collector waits for;
- `container.BindArrayType<ISaved, WalletService>()` also registers a service, but only as `ISaved`: use it for services nobody requests by their own interface. A service can also be attached directly: `saveApplier.Attach(service)`.

#### 3. Load and save

```csharp
saves.Load();        // at start-up, after all ISaved services are registered
saves.Save();        // at checkpoints, on OnApplicationPause / OnApplicationQuit
saves.RemoveSave();  // overwrite the save with an empty one
```

`Load` reads the save and calls `Load` on every registered service.

On the very first launch there is nothing under the key yet: every service receives `Load(null)` and starts with its defaults, and the initial state is written, so the next start finds a save.

`PlayerSaveService` always uses the key `"Saves"`. For several slots or profiles call `ISaveLoadReaderService` directly with your own keys:

```csharp
reader.SaveCurrentState("Slot_2");         // build SaveData from all services and write it
reader.Load("Slot_2");                     // read it and apply it to all services
SaveData data = reader.LoadData("Slot_2"); // read without applying, null if empty
reader.Save("Slot_2", data);               // write a prepared SaveData
```

`Save` and `SaveCurrentState` return `true` only when the data was actually written.

#### 4. Another storage

Derive from `BaseSaveLoadReaderService` and implement two methods; serialization, logging and applying stay in the base class:

```csharp
using System.IO;
using RedCatEngine.CommonServices.Services.Logs;
using RedCatEngine.CommonServices.Services.Save.Applier;
using RedCatEngine.CommonServices.Services.Save.SaveReader;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using UnityEngine;

public class FileSaveLoadReaderService : BaseSaveLoadReaderService
{
	[Inject]
	public FileSaveLoadReaderService(ISaveApplierContainer applier, ILogService log)
		: base(applier, log) { }

	protected override string GetJsonForLoad(string key)
	{
		var path = GetPath(key);
		return File.Exists(path) ? File.ReadAllText(path) : null;
	}

	protected override bool SetJsonForSave(string key, string json)
	{
		File.WriteAllText(GetPath(key), json);
		return true;
	}

	private static string GetPath(string key)
		=> Path.Combine(Application.persistentDataPath, key + ".json");
}
```

```csharp
container.BindType<ISaveLoadReaderService, FileSaveLoadReaderService>();
```

Return `false` from `SetJsonForSave` when writing failed, so callers of `Save` know the data was not stored.

For a game whose whole progress fits into one model class, [Application Runner](../ApplicationRunner/README.md) offers a simpler alternative: a single player model loaded at start-up and saved through `IPlayerModelSaveTrigger`.

### Tags and counted storages

```csharp
ITagStorage tags = new TagStorage();
tags.Add(bossDefeatedTag);          // TagConfig asset: Configs/Common/Tags/Tag
tags.Add(killCounterTag, 5);

if (tags.IsContain(killCounterTag, 3)) { /* at least 3 */ }
int count = tags.GetCount(killCounterTag);
```

`TagService` saves and restores an `ITagStorage` through the save system; it needs an `AllTagLinksConfig` asset listing every tag.

### Sound

```csharp
ISoundService sound = new SoundService(musicSource, sfxSource);
sound.PlaySfx(clickClip);
```

`SoundSourceContainer` is the same service as a scene component, and `SoundSfxContainer` is a component whose `ActionPlay()` can be wired to a UI button.

### Pools with DI

```csharp
container.BindType<IPoolInstanceCreatorFactory, DiPoolInstanceCreatorFactory>();
```

Pools built with this factory create instances through `IUnityGameContainer`, so pooled components receive `[MonoInject]` dependencies. See [Pools](../Pools/README.md).

### Working with DI context

Every method of the [DI container](../DependencyInjection/README.md) that creates or resolves objects — `Create`, `BindType`, `BindArrayType`, `GetSingle`, `CreateAndGetComponent`, the pool and window factories — accepts `params object[] context`. The context is a list of ready objects for this one call. They are not stored in the container; they only take part in building the object requested now.

#### Why it is needed

The container holds what is shared: services, settings, the time and log services. But many objects also need data that belongs only to them:

- a config: the window config for a window, the enemy config for an enemy, the level config for a level;
- a runtime object: the view created for this presenter, the model of this window, the state machine that creates the state;
- a primitive: the language code of a localization state, the tag of an update service.

Binding such data in the container would be wrong: there are many enemies, each with its own config, and a window config is meaningful only while the window is created. The context passes this data straight into the constructor or the `[MonoInject]` method.

```csharp
public class Enemy
{
	[Inject]
	public Enemy(EnemyConfig config, IUpdatableService updates, ILogService log) { /* ... */ }
}

var orc = container.Create<Enemy>(orcConfig);       // config from the context,
var goblin = container.Create<Enemy>(goblinConfig); // services from the container
```

#### How a parameter is matched

For every constructor (or `[MonoInject]` method) parameter the container:

1. walks the context in order and takes the **first object whose type fits the parameter** (`IsInstanceOfType`: the exact type, a base class or an implemented interface);
2. an element that is itself an `object[]` is opened, one level deep, and its items are checked the same way;
3. only if nothing fits does it search the container and its parents.

So the context always wins over the container, and the order of the context matters.

What follows from these rules:

- **Two parameters of the same type receive the same object.** `Create<Duel>(playerA, playerB)` for `Duel(Player first, Player second)` passes `playerA` twice. Give such values distinct types (a small wrapper class or a config) or pass a collection.
- **Primitives are matched by type too.** One `string` in the context goes to every `string` parameter; two strings cannot be told apart.
- **A broad type catches more than expected.** A parameter of type `object` or of a common interface takes the first context element of that type, even if it was meant for another parameter.
- **An array parameter** (`EnemyConfig[] configs`) receives an `EnemyConfig[]` element from the context as a whole, because the element itself fits the parameter.
- **Context is not inherited.** It applies to one call. Objects created later by the object you build do not see it unless that code passes it on.

#### Passing context through several levels

Infrastructure code usually takes the caller's context, adds its own objects and passes it further. This is how a window receives everything it needs: `WindowService.Open(config, context)` adds the window config and the log service, `WindowCreatorFactory` adds the created view, and the model, the view and the presenter are built from the result.

`ArrayExtensions` (namespace `RedCatEngine.CommonServices.Extensions`) keeps such code short:

| Method | Result |
|---|---|
| `context.Attach(value)` | the context plus `value` |
| `context.Attach(a, b, c)` | the context plus several values |
| `context.AttachAsSingle(array)` / `AttachAsSingle(list)` | the context plus the collection as **one** element |
| `value.WrapInArrayAsSingle()` | a new context `{ value }` |

```csharp
using RedCatEngine.CommonServices.Extensions;

public class EnemyFactory
{
	private readonly IUnityGameContainer _container;
	private readonly ILogService _log;

	[Inject]
	public EnemyFactory(IUnityGameContainer container, ILogService log)
	{
		_container = container;
		_log = log;
	}

	public EnemyView Spawn(EnemyConfig config, Transform parent, params object[] context)
	{
		// the caller's context + this enemy's config + a log tagged for this enemy
		var fullContext = context.Attach(config, _log.CreateTag(config.name));

		return _container.CreateAndGetComponent<EnemyView>(config.Prefab, parent, fullContext);
	}
}
```

`Attach` uses `Union`: a value that is already in the context is not added again, and the order is kept — the caller's objects stay first and therefore win over the objects you add. Put an object before the caller's context if yours must win:

```csharp
var fullContext = new object[] { config }.Attach(context);
```
