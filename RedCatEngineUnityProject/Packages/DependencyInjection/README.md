**English** | [Русский](README.ru.md)

# Red Cat Engine: Dependency Injection

A small reflection-based DI container. It binds services, builds objects through `[Inject]` constructors, injects `MonoBehaviour`s through `[MonoInject]` methods and supports named child scopes. Most other Red Cat Engine packages resolve their dependencies through it.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.di` |
| Version | 1.2.1 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `DependencyInjection` |
| Namespace | `RedCatEngine.DependencyInjection` |

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/DependencyInjection
```

## Dependencies

None.

## Concepts

- **`ApplicationContainer`** — pure C# container (`IApplicationContainer`).
- **`UnityGameContainer`** — adds prefab instantiation and `MonoBehaviour` injection (`IUnityGameContainer`).
- **Single / array bindings** — a type is bound either once (`BindAsSingle`) or as a collection (`BindAsArray`). Binding the same single type twice throws `BindDuplicateWithoutArrayMarkException`; use `ReBindAsSingle` to replace.
- **Context** — every `Create` / `BindType` / `GetSingle` call accepts `params object[] context`. Constructor parameters are matched against the context first, so per-call data (configs, models, payloads) does not have to be bound. See [Working with DI context](../UniversalServices/README.md#working-with-di-context).
- **Resolution order** for a constructor parameter: context → exact type in the container → any bound instance assignable to the type → parent container → auto-create and bind (concrete, non-`UnityEngine.Object` types only). If nothing fits, `NotFoundInstanceOrCreateException` is thrown.

## Usage

### Bind and resolve

```csharp
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Containers.Attributes;

public interface IWallet { int Coins { get; } }

public class Wallet : IWallet
{
	public int Coins { get; private set; }
}

public class Shop
{
	private readonly IWallet _wallet;

	[Inject] // required on a constructor with parameters
	public Shop(IWallet wallet)
		=> _wallet = wallet;
}

var container = new ApplicationContainer();

container.BindAsSingle<IWallet>(new Wallet()); // bind a ready instance
container.BindType<Shop>();                    // construct and bind

var shop = container.GetSingle<Shop>();
if (container.TryGetSingle<IWallet>(out var wallet)) { /* ... */ }
```

`BindType<TBind, TImpl>()` constructs `TImpl` and binds it as `TBind`. `Create<T>(context)` only constructs the object and does not store it.

### Arrays

```csharp
container.BindAsArray<IEnemy>(new Orc());
container.BindArrayType<IEnemy, Goblin>();

IEnumerable<IEnemy> enemies = container.GetArray<IEnemy>();
```

### Passing context

```csharp
public class Level
{
	[Inject]
	public Level(LevelConfig config, IWallet wallet) { /* ... */ }
}

// LevelConfig comes from the context, IWallet from the container
var level = container.Create<Level>(levelConfig);
```

The context is matched by type and wins over the container, so two parameters of the same type receive the same object. How matching works, what to watch out for and how to pass context through factories with `Attach`: [Working with DI context](../UniversalServices/README.md#working-with-di-context).

### Child containers

```csharp
using RedCatEngine.DependencyInjection.Specials.CreatedRule;

var levelRule = new ContainerModifyRule()
	.Add(c => c.BindType<ILevelTimer, LevelTimer>());

var levelContainer = container.CreateChildContainer("Level", levelRule);
// resolves ILevelTimer locally and IWallet from the parent
levelContainer.Dispose(); // when the scope ends
```

A `ContainerModifyRule` is a reusable list of bind actions. When it is applied, the container is also bound to itself as `ICreator`, `IBinderApplicationContainer`, `ITypeBinderApplicationContainer` and `IGetterApplicationContainer`, so services may request these narrow interfaces instead of the whole container.

### Unity objects

```csharp
using RedCatEngine.DependencyInjection.Specials.Components;

public class PlayerView : MonoConstruct
{
	private IWallet _wallet;

	[MonoInject] // required: plays the role of a constructor
	public void Construct(IWallet wallet)
		=> _wallet = wallet;

	protected override void DoInitialize() { /* after injection */ }
	protected override void DoDispose() { /* on disable */ }
}

var rule = new UnityContainerModifyRule()
	.BindType<IWallet, Wallet>();
var container = new UnityGameContainer(rule);

PlayerView view = container.CreateAndGetComponent<PlayerView>(playerPrefab, parentTransform);
```

`UnityGameContainer` instantiates the prefab, calls every `[MonoInject]` method on its `MonoConstruct` components and then `FinishInitialize()`. A `MonoConstruct` without a `[MonoInject]` method logs an error in `OnValidate`.

### Deferred dependencies

Ask for `ISingleProvider<T>` / `IArrayProvider<T>` when `T` is bound later than the consumer is created, or to break a cyclic dependency:

```csharp
using RedCatEngine.DependencyInjection.Specials.Providers;

public class Hud
{
	private readonly ISingleProvider<IPlayer> _player;

	[Inject]
	public Hud(ISingleProvider<IPlayer> player)
		=> _player = player;

	public void Refresh()
	{
		if (_player.TryGet(out var player)) { /* ... */ }
	}
}
```

## Editor tools

**Tools > RedCat DI > Container Hierarchy** shows the live container tree and what is bound in each container. Pass an `IContainerDiagnostics` implementation to the container constructor to trace its activity.
