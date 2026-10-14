[English](README.md) | **Русский**

# Red Cat Engine: Dependency Injection

Небольшой DI-контейнер на рефлексии. Регистрирует сервисы, создаёт объекты через конструкторы с `[Inject]`, внедряет зависимости в `MonoBehaviour` через методы с `[MonoInject]` и поддерживает именованные дочерние контейнеры. Через него получают зависимости большинство остальных пакетов Red Cat Engine.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.di` |
| Версия | 1.2.1 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `DependencyInjection` |
| Пространство имён | `RedCatEngine.DependencyInjection` |

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/DependencyInjection
```

## Зависимости

Нет.

## Основные понятия

- **`ApplicationContainer`** — контейнер на чистом C# (`IApplicationContainer`).
- **`UnityGameContainer`** — добавляет создание префабов и внедрение в `MonoBehaviour` (`IUnityGameContainer`).
- **Одиночные привязки и массивы** — тип привязывается либо один раз (`BindAsSingle`), либо как коллекция (`BindAsArray`). Повторная одиночная привязка того же типа бросает `BindDuplicateWithoutArrayMarkException`; чтобы заменить экземпляр, используйте `ReBindAsSingle`.
- **Контекст** — каждый вызов `Create` / `BindType` / `GetSingle` принимает `params object[] context`. Параметры конструктора сначала ищутся в контексте, поэтому данные конкретного вызова (конфиги, модели, полезную нагрузку) не нужно регистрировать в контейнере. См. [Работа с контекстом DI](../UniversalServices/README.ru.md#работа-с-контекстом-di).
- **Порядок поиска** значения для параметра конструктора: контекст → точный тип в контейнере → любой зарегистрированный экземпляр, приводимый к типу → родительский контейнер → автосоздание с регистрацией (только конкретные типы, не наследники `UnityEngine.Object`). Если ничего не подошло, бросается `NotFoundInstanceOrCreateException`.

## Использование

### Регистрация и получение

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

	[Inject] // обязателен для конструктора с параметрами
	public Shop(IWallet wallet)
		=> _wallet = wallet;
}

var container = new ApplicationContainer();

container.BindAsSingle<IWallet>(new Wallet()); // зарегистрировать готовый экземпляр
container.BindType<Shop>();                    // создать и зарегистрировать

var shop = container.GetSingle<Shop>();
if (container.TryGetSingle<IWallet>(out var wallet)) { /* ... */ }
```

`BindType<TBind, TImpl>()` создаёт `TImpl` и регистрирует его как `TBind`. `Create<T>(context)` только создаёт объект и не сохраняет его в контейнере.

### Массивы

```csharp
container.BindAsArray<IEnemy>(new Orc());
container.BindArrayType<IEnemy, Goblin>();

IEnumerable<IEnemy> enemies = container.GetArray<IEnemy>();
```

### Передача контекста

```csharp
public class Level
{
	[Inject]
	public Level(LevelConfig config, IWallet wallet) { /* ... */ }
}

// LevelConfig берётся из контекста, IWallet — из контейнера
var level = container.Create<Level>(levelConfig);
```

Контекст подбирается по типу и главнее контейнера, поэтому два параметра одного типа получат один и тот же объект. Как работает подбор, на что обратить внимание и как передавать контекст через фабрики с помощью `Attach`: [Работа с контекстом DI](../UniversalServices/README.ru.md#работа-с-контекстом-di).

### Дочерние контейнеры

```csharp
using RedCatEngine.DependencyInjection.Specials.CreatedRule;

var levelRule = new ContainerModifyRule()
	.Add(c => c.BindType<ILevelTimer, LevelTimer>());

var levelContainer = container.CreateChildContainer("Level", levelRule);
// ILevelTimer берётся из дочернего контейнера, IWallet — из родительского
levelContainer.Dispose(); // когда область больше не нужна
```

`ContainerModifyRule` — переиспользуемый список действий регистрации. При его применении контейнер также регистрирует сам себя как `ICreator`, `IBinderApplicationContainer`, `ITypeBinderApplicationContainer` и `IGetterApplicationContainer`, поэтому сервисы могут запрашивать эти узкие интерфейсы вместо всего контейнера.

### Объекты Unity

```csharp
using RedCatEngine.DependencyInjection.Specials.Components;

public class PlayerView : MonoConstruct
{
	private IWallet _wallet;

	[MonoInject] // обязателен: играет роль конструктора
	public void Construct(IWallet wallet)
		=> _wallet = wallet;

	protected override void DoInitialize() { /* после внедрения */ }
	protected override void DoDispose() { /* при отключении */ }
}

var rule = new UnityContainerModifyRule()
	.BindType<IWallet, Wallet>();
var container = new UnityGameContainer(rule);

PlayerView view = container.CreateAndGetComponent<PlayerView>(playerPrefab, parentTransform);
```

`UnityGameContainer` создаёт экземпляр префаба, вызывает все методы с `[MonoInject]` у его компонентов `MonoConstruct`, а затем `FinishInitialize()`. `MonoConstruct` без метода с `[MonoInject]` пишет ошибку в `OnValidate`.

### Отложенные зависимости

Запрашивайте `ISingleProvider<T>` / `IArrayProvider<T>`, если `T` регистрируется позже, чем создаётся потребитель, или чтобы разорвать циклическую зависимость:

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

## Инструменты редактора

Окно **Tools > RedCat DI > Container Hierarchy** показывает дерево контейнеров и то, что зарегистрировано в каждом из них. Чтобы трассировать работу контейнера, передайте в его конструктор реализацию `IContainerDiagnostics`.
