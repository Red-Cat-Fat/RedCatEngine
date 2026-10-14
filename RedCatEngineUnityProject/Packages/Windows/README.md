**English** | [Русский](README.ru.md)

# Red Cat Engine: Windows

A window (UI screen) system built on the MVP pattern: a window is described by a config asset, its model, view and presenter are created through the DI container, and a service manages layers, parent windows and opening / closing.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.windows` |
| Version | 2.0.1 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `MvpWindows` |
| Namespace | `Infrastructure.Windows` |

The assembly is named `MvpWindows`, not `Windows`: Unity's player build treats an assembly called `Windows` as WinRT metadata and breaks serialization of its types. Reference `MvpWindows` in your `.asmdef`.

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Windows
```

## Dependencies

| Package | Version | Used for |
|---|---|---|
| [Configs](../Configs/README.md) | 1.1.0 | `WindowConfig` |
| [Dependency Injection](../DependencyInjection/README.md) | 1.2.0 | creating models, views and presenters |
| [Common services](../UniversalServices/README.md) | 2.0.0 | `ILogService` |

Common services additionally requires [Pools](../Pools/README.md).

## Concepts

- **Model** (`IModel`) — the data and logic of a window. Created anew on every `Open` through the container.
- **View** (`BaseView`) — a `MonoConstruct` on the window prefab. `ActionClose()` is meant to be wired to a close button.
- **Presenter** (`BasePresenter<TView, TModel>`) — connects the model to the view in `DoOpen` / `DoClose`.
- **`WindowConfig`** — an asset with the window prefab, its `WindowLayer` and an optional parent window. `BaseModelWindowConfig<TModel, TView, TPresenter, TFactory>` binds the four types together.
- **`ILayerContainer`** — maps a `WindowLayer` (`Screen`, `Popup`, `Messages`, `Warning`, `Phrase`) to a parent transform. `LayerContainer` is the scene component.
- **`IWindowService`** — opens and closes windows.

Rules applied by `WindowService.Open`:

- the view is instantiated on first open and reused afterwards;
- a window with a parent opens the parent first;
- other open windows on the same layer are closed, except the window's parent and its related windows — see [Parent windows](#parent-windows).

## Usage

### 1. Describe a window

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

	[MonoInject] // BaseView is a MonoConstruct: the method is required
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

Create the asset, assign the window prefab (with `ShopView` on it), the layer and, if needed, the parent window.

### 2. Register the service

The scene needs a `LayerContainer` with a transform for every layer you use.

```csharp
using Infrastructure.Windows.Services;

// container is a UnityGameContainer created with a UnityContainerModifyRule, with ILogService bound
container.BindAsSingle<ILayerContainer>(layerContainer);
container.BindType<IWindowService, WindowService>();
```

### 3. Open and close

```csharp
var windows = container.GetSingle<IWindowService>();

windows.Open(shopWindowConfig);
windows.Open(itemWindowConfig, itemConfig);   // extra context for the model, view and presenter

if (windows.IsOpen(shopWindowConfig))
	windows.Close(shopWindowConfig);

windows.OpenWithCallbacks(shopWindowConfig, onOpened, onClosed);

windows.HideAll();   // close everything and remember what was open
windows.ShowAll();   // reopen it

windows.WindowOpenedEvent += config => { /* ... */ };
windows.WindowClosedEvent += config => { /* ... */ };
```

The context passed to `Open` is available to the constructors of the model and the presenter and to the `[MonoInject]` method of the view, together with the window config and `ILogService`.

## Parent windows

A window config can name a **parent** window (the *Parent* field of `WindowConfig`). The parent is the window that must be on screen behind this one: the screen a popup belongs to, the menu a sub-screen was opened from.

### Why it is needed

Without parents, every place that opens a popup has to open its screen first and in the right order. With a parent, the window carries this knowledge itself. A typical case — a popup opened from outside its screen:

```text
MainMenu (Screen)
└── Shop (Screen)            Parent: MainMenu
    └── ItemDetails (Popup)  Parent: Shop
```

The player taps a "New item!" notification during the game:

```csharp
windows.Open(itemDetailsConfig, itemConfig);
```

`ItemDetails` sees that its parent `Shop` is closed and opens it; `Shop` does the same with `MainMenu`. The player gets the whole chain — menu, shop, item card — and after closing the card is in the shop, exactly as if they had come there through the menu. The code that handles the notification knows nothing about the shop or the menu.

### How it works

When a window is opened:

1. If it has a parent and the parent is not open, the parent is opened first, with the same context. This repeats up the chain.
2. The window is remembered as a child of its parent.
3. Other open windows **on the window's layer** are closed, except:
   - for a window with a parent — the parent and the parent's other children;
   - for a window that is itself a parent — its children.
4. Windows on other layers are not touched.

So a parent and its children can share a layer, and opening a child does not close its parent; opening an unrelated screen on the same layer closes them all.

| Action | Result |
|---|---|
| `Open(itemDetails)` with everything closed | `MainMenu`, `Shop`, `ItemDetails` open |
| `Open(itemDetails)` with `Shop` already open | only `ItemDetails` opens; `Shop` stays |
| `Close(itemDetails)` | `ItemDetails` closes; `Shop` stays |
| `Open(settings)` (no parent, layer Screen) | `Shop` and `MainMenu` (Screen) close; `ItemDetails` (Popup) stays |

Notes:

- Closing a parent does not close its children. Close them explicitly, or use `HideAll()`.
- The parent receives the child's context. Its model, view and presenter must be able to take what they need from that context or the container.
- Parents must not form a cycle (`A` → `B` → `A`): there is no protection, and opening such a window never returns.
