[English](README.md) | **Русский**

# Red Cat Engine: Windows

Система окон (экранов интерфейса) на паттерне MVP: окно описывается ассетом-конфигом, его модель, представление и презентер создаются через DI-контейнер, а сервис управляет слоями, родительскими окнами и открытием / закрытием.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.windows` |
| Версия | 2.0.2 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `MvpWindows` |
| Пространство имён | `Infrastructure.Windows` |

Сборка называется `MvpWindows`, а не `Windows`: при сборке плеера Unity считает сборку с именем `Windows` метаданными WinRT и ломает сериализацию её типов. В своём `.asmdef` ссылайтесь на `MvpWindows`.

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Windows
```

## Зависимости

| Пакет | Версия | Для чего |
|---|---|---|
| [Configs](../Configs/README.ru.md) | 1.1.1 | `WindowConfig` |
| [Dependency Injection](../DependencyInjection/README.ru.md) | 1.2.1 | создание моделей, представлений и презентеров |
| [Common services](../UniversalServices/README.ru.md) | 2.0.1 | `ILogService` |

Пакету Common services дополнительно нужен [Pools](../Pools/README.ru.md).

## Основные понятия

- **Модель** (`IModel`) — данные и логика окна. Создаётся заново при каждом `Open` через контейнер.
- **Представление** (`BaseView`) — `MonoConstruct` на префабе окна. Метод `ActionClose()` предназначен для привязки к кнопке закрытия.
- **Презентер** (`BasePresenter<TView, TModel>`) — связывает модель с представлением в `DoOpen` / `DoClose`.
- **`WindowConfig`** — ассет с префабом окна, его слоем `WindowLayer` и необязательным родительским окном. `BaseModelWindowConfig<TModel, TView, TPresenter, TFactory>` связывает четыре типа вместе.
- **`ILayerContainer`** — сопоставляет слой `WindowLayer` (`Screen`, `Popup`, `Messages`, `Warning`, `Phrase`) с родительским трансформом. `LayerContainer` — компонент сцены.
- **`IWindowService`** — открывает и закрывает окна.

Правила, которые применяет `WindowService.Open`:

- представление создаётся при первом открытии и затем переиспользуется;
- окно с родителем сначала открывает родителя;
- остальные открытые окна на том же слое закрываются, кроме родителя окна и связанных с ним окон — см. [Родительские окна](#родительские-окна).

## Использование

### 1. Опишите окно

```csharp
using Infrastructure.Windows.Components;
using Infrastructure.Windows.Components.Windows;
using Infrastructure.Windows.Factory;
using Infrastructure.Windows.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using UnityEngine;

public class ShopModel : IModel
{
	public int Coins { get; }

	[Inject]
	public ShopModel(IWallet wallet)
		=> Coins = wallet.Coins;
}

public class ShopView : BaseView
{
	[SerializeField] private UnityEngine.UI.Text _coins;

	[MonoInject] // BaseView — это MonoConstruct: метод обязателен
	public void Construct() { }

	public void SetCoins(int coins) => _coins.text = coins.ToString();
}

public class ShopPresenter : BasePresenter<ShopView, ShopModel>
{
	[Inject]
	public ShopPresenter(ShopView view) : base(view) { }

	protected override void DoOpen(ShopModel model) => View.SetCoins(model.Coins);
	protected override void DoClose() { }
}

[CreateAssetMenu(menuName = "Configs/Windows/Shop")]
public class ShopWindowConfig : BaseModelWindowConfig<
	ShopModel,
	ShopView,
	ShopPresenter,
	WindowCreatorFactory<ShopView, ShopPresenter>> { }
```

Создайте ассет, назначьте префаб окна (с компонентом `ShopView`), слой и, если нужно, родительское окно.

### 2. Зарегистрируйте сервис

На сцене должен быть `LayerContainer` с трансформом для каждого используемого слоя.

```csharp
using Infrastructure.Windows.Services;

// container — UnityGameContainer, созданный с UnityContainerModifyRule; ILogService уже зарегистрирован
container.BindAsSingle<ILayerContainer>(layerContainer);
container.BindType<IWindowService, WindowService>();
```

### 3. Открывайте и закрывайте

```csharp
var windows = container.GetSingle<IWindowService>();

windows.Open(shopWindowConfig);
windows.Open(itemWindowConfig, itemConfig);   // дополнительный контекст для модели, представления и презентера

if (windows.IsOpen(shopWindowConfig))
	windows.Close(shopWindowConfig);

windows.OpenWithCallbacks(shopWindowConfig, onOpened, onClosed);

windows.HideAll();   // закрыть всё и запомнить, что было открыто
windows.ShowAll();   // открыть заново

windows.WindowOpenedEvent += config => { /* ... */ };
windows.WindowClosedEvent += config => { /* ... */ };
```

Контекст, переданный в `Open`, доступен конструкторам модели и презентера и методу с `[MonoInject]` у представления — вместе с конфигом окна и `ILogService`.

## Родительские окна

В конфиге окна можно указать **родительское** окно (поле *Parent* у `WindowConfig`). Родитель — это окно, которое должно быть на экране за текущим: экран, к которому относится всплывающее окно, или меню, из которого открыт подэкран.

### Зачем это нужно

Без родителей каждое место, открывающее всплывающее окно, должно сначала открыть его экран, причём в правильном порядке. С родителем это знание хранит само окно. Типичный случай — всплывающее окно, открытое не из своего экрана:

```text
MainMenu (Screen)
└── Shop (Screen)            Parent: MainMenu
    └── ItemDetails (Popup)  Parent: Shop
```

Во время игры игрок нажимает на уведомление «Новый предмет!»:

```csharp
windows.Open(itemDetailsConfig, itemConfig);
```

`ItemDetails` видит, что его родитель `Shop` закрыт, и открывает его; `Shop` делает то же самое с `MainMenu`. Игрок получает всю цепочку — меню, магазин, карточку предмета — и после закрытия карточки оказывается в магазине, как если бы пришёл туда через меню. Код, обрабатывающий уведомление, ничего не знает ни о магазине, ни о меню.

### Как это работает

При открытии окна:

1. Если у окна есть родитель и он не открыт, сначала открывается родитель с тем же контекстом. Это повторяется вверх по цепочке.
2. Окно запоминается как дочернее для своего родителя.
3. Остальные открытые окна **на слое этого окна** закрываются, кроме:
   - для окна с родителем — самого родителя и других дочерних окон этого родителя;
   - для окна, которое само является родителем, — его дочерних окон.
4. Окна на других слоях не затрагиваются.

Поэтому родитель и дочерние окна могут находиться на одном слое, и открытие дочернего окна не закрывает родителя; а открытие постороннего экрана на том же слое закрывает их все.

| Действие | Результат |
|---|---|
| `Open(itemDetails)`, всё закрыто | открыты `MainMenu`, `Shop`, `ItemDetails` |
| `Open(itemDetails)`, `Shop` уже открыт | открывается только `ItemDetails`; `Shop` остаётся |
| `Close(itemDetails)` | `ItemDetails` закрывается; `Shop` остаётся |
| `Open(settings)` (без родителя, слой Screen) | `Shop` и `MainMenu` (Screen) закрываются; `ItemDetails` (Popup) остаётся |

Примечания:

- Закрытие родителя не закрывает дочерние окна. Закрывайте их явно или используйте `HideAll()`.
- Родитель получает контекст дочернего окна. Его модель, представление и презентер должны брать нужное из этого контекста или из контейнера.
- Родители не должны образовывать цикл (`A` → `B` → `A`). Если цикл есть, открытие останавливается на окне, которое уже открывается, и в лог пишется ошибка.
