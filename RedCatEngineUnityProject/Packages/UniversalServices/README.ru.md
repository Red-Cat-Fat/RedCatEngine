[English](README.md) | **Русский**

# Red Cat Engine: Common services

Набор сервисов общего назначения и вспомогательных типов, которыми пользуются игры и другие пакеты Red Cat Engine: логи, время, цикл обновления, отложенные вызовы, случайные числа, сохранения, теги, звук, хранилища и методы расширения.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.services` |
| Версия | 2.0.1 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `CommonServices` |
| Пространство имён | `RedCatEngine.CommonServices` |

Папка называется `UniversalServices`, а сборка и пространство имён — `CommonServices`.

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/UniversalServices
```

## Зависимости

| Пакет | Версия | Для чего |
|---|---|---|
| [Configs](../Configs/README.ru.md) | 1.1.1 | `TagConfig`, `BaseItemConfig`, `TimeSettingsConfig` |
| [Dependency Injection](../DependencyInjection/README.ru.md) | 1.2.1 | конструкторы с `[Inject]`, компоненты `MonoConstruct` |
| [Pools](../Pools/README.ru.md) | 1.1.0 | `DiPoolInstanceCreatorFactory` |

Опционально: `DoTweenExtensions` компилируется только при заданном символе компиляции `DOTWEEN` (установлен DOTween).

## Что внутри

| Область | Основные типы |
|---|---|
| Логи | `ILogService`, `LogHub`, `LogRouter`, `LogLevelFilter`, `FileLogSink`, `UnityConsoleSink` |
| Время | `ITimeService`, `IPausedTimeService`, `UnityTimeService` |
| Цикл обновления | `IUpdatableService`, `UpdatableService`, `UnityUpdater`, `MonoConstructUpdatable` |
| Отложенные вызовы | `IDisposableCallbackService`, `DisposableCallback`, `DisposableTimerCallback` |
| Случайные числа | `IRandomService`, `UnityRandomService` |
| Сохранения | `ISaved`, `ISaveApplierContainer`, `ISaveLoadReaderService`, `PlayerSaveService` |
| Теги | `TagConfig`, `ITagStorage`, `TagService` |
| Звук | `ISoundService`, `SoundService`, `SoundSourceContainer`, `SoundSfxContainer` |
| Курсор | `IMouseLockerService` |
| Хранилища | `BaseStorage<T>`, `BaseCountedStorage<T>`, `TypedStorage`, `RedComponentContainer`, `Observable<T>` |
| Специальные типы | `ChangeValue<T>`, `DeltaChangeValue<T>`, `ChangeTrackedValue<T>`, `UnityTag`, `ComponentCollector<T>` |
| Пулы + DI | `DiPoolInstanceCreatorFactory`, `DiInstanceCreator` |
| Методы расширения | массивы, строки, векторы, `AnimationCurve`, `LayerMask`, гизмо |

## Использование

### Логи

Сервис логов пишет через маршрутизатор в один или несколько приёмников; фильтр решает, какие уровни пропускать — глобально и для отдельных тегов.

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
		=> _log = log.CreateTag<EnemySpawner>(); // каждое сообщение получает тег

	public void Spawn(string id)
	{
		_log.Info("Spawn enemy {0}", id);
		_log.LogWarning("Spawn point is busy");
	}
}
```

Вызовы `Verbose` / `Info` компилируются только в редакторе, в development-сборках или с символом `CHEAT_ENABLED`; предупреждения и ошибки пишутся всегда. Код, у которого нет контейнера, может использовать `LogHub.CreateTag<T>()`.

#### Иерархия тегов

`CreateTag` возвращает новый сервис логов, тег которого дописывается через `:` к тегу сервиса, из которого он создан. Каждая система создаёт тег от полученного лога, и теги повторяют структуру игры:

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

Консоль Unity:

```text
[Game:Battle:EnemySpawner] Spawn enemy Orc
[Game:Battle:Waves] Wave 3 started
[Game:UI] Open window Shop
```

Файл лога (`FileLogSink`) — с локальным временем, уровнем и номером потока:

```text
[14:02:11.384] [INFO] [T1] [Game:Battle:EnemySpawner] Spawn enemy Orc
[14:02:11.390] [INFO] [T1] [Game:Battle:Waves] Wave 3 started
[14:02:12.007] [INFO] [T1] [Game:UI] Open window Shop
```

По тегу вывод удобно фильтровать: поиск `Game:Battle` в консоли или в файле оставит только бой.

Фильтр уровней работает по той же иерархии. Уровень, заданный для тега, действует на все теги под ним, а побеждает самая точная настройка:

```csharp
filter.SetGlobalLevel(LogLevel.Warning);              // всё: только предупреждения и ошибки
filter.SetLevel("Game:Battle", LogLevel.Verbose);     // весь бой: всё
filter.SetLevel("Game:Battle:Waves", LogLevel.Off);   // кроме волн: ничего
```

| Тег | Действующий уровень |
|---|---|
| `Game:UI` | Warning (глобальный) |
| `Game:Battle:EnemySpawner` | Verbose (от `Game:Battle`) |
| `Game:Battle:EnemySpawner:Orc_01` | Verbose (от `Game:Battle`) |
| `Game:Battle:Waves` | Off |

`CreateTag(string)` и `CreateTag<T>()` регистрируют тег в фильтре, поэтому он появляется в дереве тегов окна **Tools > 🐛 Log Filter**, где уровни можно менять прямо во время игры. `CreateTag(GameObject)` / `CreateInstanceTag(name)` предназначены для логов отдельных объектов: они фильтруются по настройкам родителя, но в дерево не добавляются. Изменения уровней сохраняет `ILogFilterStorage`, переданный в `LogLevelFilter`. Окно **Tools > Log Viewer** открывает файлы логов.

### Время и цикл обновления

`UnityTimeService` — компонент сцены, реализующий `ITimeService` (время кадра, фиксированный шаг, масштаб времени, общее время) и `IPausedTimeService`:

```csharp
container.BindAsSingle<ITimeService>(unityTimeService);
container.BindAsSingle<IPausedTimeService>(unityTimeService);
```

Пауза учитывается по источникам — `pausedTime.Pause(this)` / `pausedTime.UnPause(this)`, — и время стоит на паузе, пока её удерживает хотя бы один источник.

#### Update для классов, которые не являются MonoBehaviour

В Unity вызовы `Update`, `FixedUpdate` и `LateUpdate` получает только `MonoBehaviour`. А игровая логика обычно живёт в обычных классах C#, которые создаёт DI-контейнер: сервисах, контроллерах, ИИ, таймерах. Без цикла обновления такому классу понадобился бы вспомогательный `MonoBehaviour`, только чтобы пересылать ему `Update`, — или пришлось бы самому стать `MonoBehaviour`.

`IUpdatableService` даёт обычным классам те же три вызова:

| Интерфейс | Метод | Вызывается из |
|---|---|---|
| `IUpdatable` | `OnUpdate()` | `Update` |
| `IFixedUpdatable` | `OnFixedUpdate()` | `FixedUpdate` |
| `ILateUpdatable` | `OnLateUpdate()` | `LateUpdate` |

Один компонент `UnityUpdater` на сцене (он реализует `IUpdateTrigger`) получает вызовы Unity и передаёт их в `UpdatableService`, а тот вызывает все зарегистрированные объекты. На сцене остаётся один `MonoBehaviour` с `Update` вместо отдельного на каждую систему.

Зарегистрируйте сервис один раз:

```csharp
using RedCatEngine.CommonServices.Services.UpdatableService;

container.BindAsSingle<IUpdateTrigger>(unityUpdater);
container.BindType<IUpdatableService, UpdatableService>("Game"); // "Game" — тег в сообщениях об ошибках
```

Класс подписывается, когда начинает работу, и отписывается, когда заканчивает. У `OnUpdate` нет параметров; время кадра берите из `ITimeService` — тогда класс учитывает паузу и масштаб времени игры:

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

DI-контейнер при уничтожении вызывает `Dispose` у созданных им объектов, поэтому отписываться нужно в `Dispose`. Объект, который не отписался, продолжает обновляться и не освобождается сборщиком мусора.

Как работает `UpdatableService`:

- объекты вызываются в порядке добавления; повторное добавление того же объекта ничего не делает;
- добавление и удаление во время обновления применяются после текущего прохода, поэтому объект может безопасно удалить себя в `OnUpdate`;
- если объект бросает исключение, оно пишется в лог через `LogHub` с тегом сервиса и описанием объекта (для компонента — путь к `GameObject`) и пробрасывается дальше, так что остальные объекты в этом кадре не вызываются;
- `Dispose()` отписывает сервис от `IUpdateTrigger` и очищает все списки.

Можно зарегистрировать несколько сервисов с разными тегами в разных контейнерах — например, по одному на уровень — и уничтожать их вместе с этой областью.

#### Компоненты

Компонент, созданный DI-контейнером, может наследоваться от `MonoConstructUpdatable` или `MonoConstructLateUpdatable`. Он получает `IUpdatableService` и `ITimeService` через `[MonoInject]`, регистрируется после инициализации, отписывается при уничтожении и получает время кадра параметром:

```csharp
public class Rotator : MonoConstructUpdatable
{
	protected override void DoUpdate(float deltaTime)
		=> transform.Rotate(0f, 90f * deltaTime, 0f);

	protected override void DoSubscribeAfterInitialize() { /* вместо DoInitialize */ }
	protected override void DoUnsubscribeAfterDispose() { /* вместо DoDispose */ }
}
```

`TestedMonoConstructUpdatable` — вариант для отладочных компонентов: в редакторе он обновляется, а в сборке один раз пишет ошибку в лог вместо обновления.

### Отложенные вызовы

`IDisposableCallbackService` создаёт колбэки, которые выполняются не больше одного раза и управляются возвращённым вам объектом — **токеном**. Через токен колбэк можно отменить (`Dispose()`), выполнить досрочно (`Invoke()`) или проверить, ждёт ли он ещё выполнения (`IsEnable`).

```csharp
using RedCatEngine.CommonServices.Services.DisposableCallbacks;

container.BindType<IDisposableCallbackService, DisposableCallbackService>(); // нужен ITimeService
```

#### Виды колбэков

| Метод | Токен | Когда выполняется | При `Dispose()` до выполнения |
|---|---|---|---|
| `MakeTimerCallback(time, callback)` | `DisposableTimerCallback` | через `time` секунд | отменяется, не выполняется |
| `MakeAlwaysInvokeTimerCallback(time, callback)` | `DisposableTimerCallback` | через `time` секунд | **выполняется сразу** |
| `MakeCallback(callback)` | `DisposableCallback` | при вызове `Invoke()` | отменяется, не выполняется |
| `MakeAlwaysInvokeCallback(callback)` | `DisposableCallback` | при вызове `Invoke()` | **выполняется сразу** |
| `MakeCallback<TResult>(callback)` | `DisposableCallback<TResult>` | при вызове `Invoke(result)` | отменяется, не выполняется |

В любом случае колбэк выполняется **не больше одного раза**: после `Invoke()` (или срабатывания таймера) повторные `Invoke()` и `Dispose()` ничего не делают.

Таймеры считают время, приходящее в `ITimeService.UpdateEvent`. С `UnityTimeService` это масштабированный `Time.deltaTime`, поэтому таймеры замедляются вместе с игрой, в том числе на паузе. `TimeLeft` показывает, сколько секунд осталось.

#### Отменяемые таймеры

Подходят для действий, которые должны произойти позже, только если за это время ничего не изменилось:

```csharp
// скрыть подсказку через 3 секунды, если игрок не закроет её раньше
_hideHint = _callbacks.MakeTimerCallback(3f, () => _hint.Hide());

public void OnHintClosedByPlayer()
{
	_hideHint?.Dispose(); // таймер отменён, Hide() не вызовется
	_hint.Hide();
}
```

#### Колбэки, которые выполняются всегда

Варианты *always invoke* гарантируют, что колбэк выполнится ровно один раз: при срабатывании таймера или вызове `Invoke()` — или, в крайнем случае, при уничтожении токена. Используйте их для очистки, которую нельзя потерять, как блок `finally`:

```csharp
_pause.Pause(this);
// снять паузу через 2 секунды — или раньше, если токен уничтожат
_resumeGame = _callbacks.MakeAlwaysInvokeTimerCallback(2f, () => _pause.UnPause(this));

public void Skip()
	=> _resumeGame.Dispose(); // UnPause выполняется сейчас, а не через 2 секунды
```

```csharp
// экран загрузки закроется, чем бы ни закончилась загрузка
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

Помните, что уничтожение токена *always invoke* **выполняет** колбэк. Не используйте эти варианты для действий, которые не должны происходить при отмене, и следите, чтобы всё, к чему обращается колбэк, в этот момент ещё существовало.

#### Одноразовый колбэк с результатом

`MakeCallback<TResult>` превращает договорённость «ответить один раз» в токен — например, для диалога:

```csharp
public DisposableCallback<bool> AskConfirmation(Action<bool> onAnswer)
{
	_answer = _callbacks.MakeCallback(onAnswer);
	_dialog.Show();
	return _answer;
}

public void OnYes() => _answer.Invoke(true);  // засчитывается первый ответ
public void OnNo() => _answer.Invoke(false);  // последующие игнорируются
```

Вызывающая сторона может уничтожить полученный токен, чтобы перестать ждать ответа.

#### Хранение токенов

Токен — единственный способ управлять колбэком, поэтому храните его в поле, пока колбэк может выполниться:

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

		_reload?.Dispose();                                   // освободить старый токен перед заменой
		_reload = _callbacks.MakeTimerCallback(1.5f, OnReloaded);
	}

	public void Dispose()
		=> _reload?.Dispose();                                // турели больше нет: отменить перезарядку

	private void OnReloaded() { /* ... */ }
}
```

Правила, которые делают работу с токенами безопасной:

- **Уничтожайте токены вместе с владельцем** — в `Dispose()` класса или в `DoDispose()` / `OnDestroy()` компонента. Иначе таймер может сработать после уничтожения владельца и обратиться к уничтоженному объекту.
- **Освобождайте старый токен перед заменой**, как в `Fire()` выше, иначе старый таймер продолжит работать.
- **Несколько токенов одного владельца** можно хранить в `List<IDisposable>` и уничтожать в цикле.
- **Проверяйте `IsEnable`**, чтобы узнать, ждёт ли колбэк выполнения; после выполнения или отмены он становится `false`.
- Уничтожение `DisposableCallbackService` уничтожает все созданные им токены: обычные колбэки отменяются, колбэки *always invoke* выполняются. Поэтому сервис, зарегистрированный в дочернем контейнере (например, на уровень), очищает свои колбэки вместе с уровнем.
- Выполненные и отменённые колбэки сервис освобождает, поэтому множество короткоживущих колбэков не накапливается.

### Случайные числа

```csharp
IRandomService random = new UnityRandomService();
random.SetSeed(42);

int index = random.GetRange(0, 10);          // верхняя граница не включается
var enemy = random.GetRandomElement(enemies);
var order = random.GetShuffle(cards);
float damage = random.ErrorRate(100f, 0.1f);
```

### Сохранения

Система сохранений собирает состояние всех сервисов, которые нужно сохранять, в одно сохранение, записывает его под ключом, а при загрузке возвращает каждому сервису его часть. Сервисы не знают, где и как хранится сохранение, а хранилище не знает, что содержат сервисы.

#### Как это работает

```text
          Save()                                        Load()
            │                                             │
   PlayerSaveService (ключ "Saves")              PlayerSaveService
            │                                             │
   ISaveLoadReaderService ── JSON ──► хранилище ── JSON ──► ISaveLoadReaderService
            │   (PlayerPrefs, файл, облако…)               │
   ISaveApplierContainer                           ISaveApplierContainer
     GenerateSaveWithName(key)                       Load(SaveData)
            │                                             │
   каждый ISaved: GetSerializedSaveData()        каждый ISaved: Load(свой json)
```

Сохранение — это `SaveData`: имя и массив записей `ServiceSaveData`, по одной на сервис, — `SaveKey` сервиса и строка, которую он вернул. Каждый сервис сам сериализует свои данные; затем сервис чтения сериализует весь `SaveData` через `JsonUtility` и сохраняет полученную строку.

#### Составные части

| Тип | Роль | Что с ним делать |
|---|---|---|
| `ISaved` | Сервис, состояние которого сохраняется: `SaveKey`, `GetSerializedSaveData()`, `Load(string)` | **Реализовать** в каждом сервисе с сохраняемым состоянием |
| `SaveSerializer<TData>` | Помощник `JsonUtility` для класса данных сервиса; для пустой строки возвращает новый `TData` | Использовать внутри реализаций `ISaved` |
| `ISaveApplierContainer` / `SaveApplierContainer` | Собирает сервисы `ISaved`, строит из них `SaveData` и раздаёт записи при загрузке | Зарегистрировать один раз |
| `ISaveLoadReaderService` / `BaseSaveLoadReaderService` | Читает и пишет JSON `SaveData` под ключом; пишет в лог каждую операцию | Зарегистрировать одну реализацию; для другого хранилища — **унаследоваться** |
| `PlayerPrefSaveLoadReaderService` | Хранилище в `PlayerPrefs` | Готов к использованию |
| `PlayerSaveService` | Сохраняет, загружает и стирает сохранение под фиксированным ключом `"Saves"` | Вызывать из игрового кода |

#### 1. Сделайте сервис сохраняемым

Храните данные в отдельном классе с `[Serializable]` и ссылайтесь на конфиги через `ConfigID`, а не на сам ассет:

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

	public string SaveKey => "Wallet";   // уникальный и неизменный: при переименовании старые сохранения теряются

	public int Coins => _data.Coins;

	public void Add(int coins) => _data.Coins += coins;

	public string GetSerializedSaveData()
		=> _serializer.SaveDataFrom(_data);

	public void Load(string json)
		=> _data = _serializer.LoadDataFrom(json); // json равен null, если в сохранении нет записи с этим ключом
}
```

`Load` получает `null`, если для сервиса в сохранении нет записи, — при первом запуске или когда сервис появился в новой версии игры. `SaveSerializer.LoadDataFrom` в этом случае возвращает новый объект данных, и сервис просто начинает со значений по умолчанию.

#### 2. Зарегистрируйте сервисы

`SaveApplierContainer` получает сервисы `ISaved` через DI-контейнер: он ожидает массив `ISaved`, поэтому к нему присоединяется каждый экземпляр, зарегистрированный **после него** через `BindAsArray<ISaved>`.

```csharp
using RedCatEngine.CommonServices.Services.Save;
using RedCatEngine.CommonServices.Services.Save.Applier;
using RedCatEngine.CommonServices.Services.Save.Realisations;
using RedCatEngine.CommonServices.Services.Save.SaveReader;

// 1. сборщик — раньше любого сервиса ISaved
container.BindType<ISaveApplierContainer, SaveApplierContainer>();

// 2. хранилище и фасад
container.BindType<ISaveLoadReaderService, PlayerPrefSaveLoadReaderService>(); // нужен ILogService
var saves = container.BindType<PlayerSaveService>();

// 3. сервисы: регистрируем как обычно, затем каждый — как ISaved
var wallet = container.BindType<IWallet, WalletService>();
container.BindAsArray<ISaved>((ISaved)wallet);

var tags = container.BindType<ITagService, TagService>();
container.BindAsArray<ISaved>(tags);
```

Здесь важны две детали:

- аргумент-тип `BindAsArray` должен быть именно `ISaved` — по этому ключу сборщик ждёт сервисы;
- `container.BindArrayType<ISaved, WalletService>()` тоже регистрирует сервис, но только как `ISaved`: подходит для сервисов, которые никто не запрашивает по их собственному интерфейсу. Сервис можно присоединить и напрямую: `saveApplier.Attach(service)`.

#### 3. Загрузка и сохранение

```csharp
saves.Load();        // при запуске, после регистрации всех сервисов ISaved
saves.Save();        // на контрольных точках, в OnApplicationPause / OnApplicationQuit
saves.RemoveSave();  // перезаписать сохранение пустым
```

`Load` читает сохранение и вызывает `Load` у каждого зарегистрированного сервиса.

При самом первом запуске под ключом ещё ничего нет: каждый сервис получает `Load(null)` и начинает со значений по умолчанию, а начальное состояние записывается, чтобы при следующем запуске сохранение уже было.

`PlayerSaveService` всегда использует ключ `"Saves"`. Для нескольких слотов или профилей вызывайте `ISaveLoadReaderService` напрямую со своими ключами:

```csharp
reader.SaveCurrentState("Slot_2");         // собрать SaveData со всех сервисов и записать
reader.Load("Slot_2");                     // прочитать и применить ко всем сервисам
SaveData data = reader.LoadData("Slot_2"); // прочитать без применения, null — если пусто
reader.Save("Slot_2", data);               // записать готовый SaveData
```

`Save` и `SaveCurrentState` возвращают `true`, только если данные действительно записаны.

#### 4. Другое хранилище

Унаследуйтесь от `BaseSaveLoadReaderService` и реализуйте два метода; сериализация, логирование и применение остаются в базовом классе:

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

Возвращайте `false` из `SetJsonForSave`, если запись не удалась, — тогда вызывающие `Save` узнают, что данные не сохранены.

Для игры, весь прогресс которой помещается в один класс модели, [Application Runner](../ApplicationRunner/README.ru.md) предлагает более простой вариант: одна модель игрока, загружаемая при запуске и сохраняемая через `IPlayerModelSaveTrigger`.

### Теги и хранилища со счётчиком

```csharp
ITagStorage tags = new TagStorage();
tags.Add(bossDefeatedTag);          // ассет TagConfig: Configs/Common/Tags/Tag
tags.Add(killCounterTag, 5);

if (tags.IsContain(killCounterTag, 3)) { /* не меньше 3 */ }
int count = tags.GetCount(killCounterTag);
```

`TagService` сохраняет и восстанавливает `ITagStorage` через систему сохранений; ему нужен ассет `AllTagLinksConfig` со списком всех тегов.

### Звук

```csharp
ISoundService sound = new SoundService(musicSource, sfxSource);
sound.PlaySfx(clickClip);
```

`SoundSourceContainer` — тот же сервис в виде компонента сцены, а `SoundSfxContainer` — компонент, метод `ActionPlay()` которого можно привязать к кнопке интерфейса.

### Пулы с DI

```csharp
container.BindType<IPoolInstanceCreatorFactory, DiPoolInstanceCreatorFactory>();
```

Пулы, созданные с этой фабрикой, создают экземпляры через `IUnityGameContainer`, поэтому компоненты объектов пула получают зависимости через `[MonoInject]`. См. [Pools](../Pools/README.ru.md).

### Работа с контекстом DI

Каждый метод [DI-контейнера](../DependencyInjection/README.ru.md), который создаёт или находит объекты, — `Create`, `BindType`, `BindArrayType`, `GetSingle`, `CreateAndGetComponent`, фабрики пулов и окон — принимает `params object[] context`. Контекст — это список готовых объектов для одного вызова. Они не сохраняются в контейнере, а только участвуют в построении объекта, запрошенного сейчас.

#### Зачем он нужен

В контейнере лежит общее: сервисы, настройки, сервисы времени и логов. Но многим объектам нужны ещё и данные, принадлежащие только им:

- конфиг: конфиг окна для окна, конфиг врага для врага, конфиг уровня для уровня;
- объект времени выполнения: представление, созданное для этого презентера, модель этого окна, машина состояний, создающая состояние;
- примитив: код языка для состояния локализации, тег для сервиса обновления.

Регистрировать такие данные в контейнере неправильно: врагов много, у каждого свой конфиг, а конфиг окна имеет смысл только пока окно создаётся. Контекст передаёт эти данные прямо в конструктор или в метод с `[MonoInject]`.

```csharp
public class Enemy
{
	[Inject]
	public Enemy(EnemyConfig config, IUpdatableService updates, ILogService log) { /* ... */ }
}

var orc = container.Create<Enemy>(orcConfig);       // конфиг — из контекста,
var goblin = container.Create<Enemy>(goblinConfig); // сервисы — из контейнера
```

#### Как подбирается параметр

Для каждого параметра конструктора (или метода с `[MonoInject]`) контейнер:

1. проходит контекст по порядку и берёт **первый объект, тип которого подходит к параметру** (`IsInstanceOfType`: тот же тип, родительский класс или реализованный интерфейс);
2. элемент, который сам является `object[]`, раскрывается на один уровень, и его элементы проверяются так же;
3. только если ничего не подошло, ищет в контейнере и его родителях.

Поэтому контекст всегда главнее контейнера, а порядок элементов контекста важен.

Что из этого следует:

- **Два параметра одного типа получат один и тот же объект.** `Create<Duel>(playerA, playerB)` для `Duel(Player first, Player second)` передаст `playerA` дважды. Дайте таким значениям разные типы (небольшой класс-обёртка или конфиг) или передайте коллекцию.
- **Примитивы тоже подбираются по типу.** Одна строка `string` в контексте попадёт во все параметры типа `string`; две строки различить нельзя.
- **Широкий тип забирает больше ожидаемого.** Параметр типа `object` или общего интерфейса возьмёт первый подходящий элемент контекста, даже если тот предназначался другому параметру.
- **Параметр-массив** (`EnemyConfig[] configs`) получает элемент `EnemyConfig[]` из контекста целиком, потому что сам элемент подходит к параметру.
- **Контекст не наследуется.** Он действует в одном вызове. Объекты, которые позже создаст построенный объект, его не видят, если этот код не передаст контекст дальше.

#### Передача контекста через несколько уровней

Инфраструктурный код обычно берёт контекст вызывающего, добавляет свои объекты и передаёт дальше. Так окно получает всё необходимое: `WindowService.Open(config, context)` добавляет конфиг окна и сервис логов, `WindowCreatorFactory` — созданное представление, а модель, представление и презентер строятся из результата.

`ArrayExtensions` (пространство имён `RedCatEngine.CommonServices.Extensions`) делает такой код коротким:

| Метод | Результат |
|---|---|
| `context.Attach(value)` | контекст плюс `value` |
| `context.Attach(a, b, c)` | контекст плюс несколько значений |
| `context.AttachAsSingle(array)` / `AttachAsSingle(list)` | контекст плюс коллекция **одним** элементом |
| `value.WrapInArrayAsSingle()` | новый контекст `{ value }` |

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
		// контекст вызывающего + конфиг этого врага + лог с тегом этого врага
		var fullContext = context.Attach(config, _log.CreateTag(config.name));

		return _container.CreateAndGetComponent<EnemyView>(config.Prefab, parent, fullContext);
	}
}
```

`Attach` использует `Union`: значение, которое уже есть в контексте, повторно не добавляется, а порядок сохраняется — объекты вызывающего остаются первыми и поэтому побеждают добавленные вами. Если ваш объект должен победить, поставьте его перед контекстом вызывающего:

```csharp
var fullContext = new object[] { config }.Attach(context);
```
