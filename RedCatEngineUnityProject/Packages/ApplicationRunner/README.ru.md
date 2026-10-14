[English](README.md) | **Русский**

# Red Cat Engine: Application runner

Заготовки для запуска игры: машина состояний, состояния которой создаёт DI-контейнер, готовые стартовые состояния (локализация, читы, загрузка модели игрока) и контейнер модели игрока с триггерами сохранения.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.aplication.runner` |
| Версия | 1.1.0 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `ApplicationRunner` |
| Пространство имён | `RedCatEngine.ApplicationRunner` |

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/ApplicationRunner
```

## Зависимости

| Пакет | Версия | Для чего |
|---|---|---|
| [Dependency Injection](../DependencyInjection/README.ru.md) | 1.2.0 | создание состояний, регистрация модели игрока |
| [State Machine](../StateMachine/README.ru.md) | 2.0.0 | `BaseTypedStateMachine`, интерфейсы состояний |
| [Localization](https://github.com/elmortem/localization) | git | `LocalizeSystem` в состояниях локализации — добавляется в проект вручную |

Опционально: `YandexInitializeLocalizationState` и `YandexModelLoader` компилируются только с символом компиляции `YANDEX_GAME` и требуют плагин YandexGame.

## Основные понятия

- **`TypeBasedStateMachine<TBaseState>`** — [машина состояний](../StateMachine/README.ru.md), которая создаёт состояния через `ICreator`. Состояние можно зарегистрировать под интерфейсом или родительским классом (`AddState<TTag, TImpl>(context)`, см. [регистрацию под базовым типом](../StateMachine/README.ru.md#регистрация-состояния-под-базовым-типом)), а незарегистрированное состояние создаётся при первом `Enter`.
- **Стартовые состояния** — каждое делает свою работу и вызывает `EnterNextFromQueue()`:
  - `IInitializeLocalizationState` — `EditorLocalizationState` (фиксированный язык) или `YandexInitializeLocalizationState` (язык окружения Яндекса). Поддерживаемые коды: `ru`, `en`, `tr`, `fr`, `de`; для остальных используется английский.
  - `ILoadPlayerModelState` — `LoadPlayerModelState<TModel, TLoader>` загружает модель игрока и регистрирует её в контейнере.
  - `IInitializeCheatState` — наследуйтесь от `BaseInitializeCheatState`, чтобы создать префаб чит-консоли.
- **Модель игрока** — `IPlayerModelContainer<TModel>` хранит модель и сохраняет её; `IModelLoader<TModel>` читает и записывает её (`EditorModelLoader` использует `PlayerPrefs`, `YandexModelLoader` — облачные сохранения Яндекса); `IPlayerModelSaveTrigger.Save()` запрашивает сохранение из любого места.

## Использование

### Машина состояний игры

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
		// `this` и "en" передаются в конструктор состояния как контекст
		AddState<IInitializeLocalizationState, EditorLocalizationState>(this, "en");
		AddState<ILoadPlayerModelState, LoadPlayerModelState<PlayerModel, EditorModelLoader<PlayerModel>>>(this);
	}
}

public class GameLoopState : IState
{
	private readonly PlayerModel _model;

	[Inject]
	public GameLoopState(PlayerModel model) // зарегистрирована состоянием загрузки
		=> _model = model;

	public void Enter() { /* запустить игру */ }
	public void Exit() { }
}
```

### Запуск

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
			.AddToQueue<GameLoopState>()   // не зарегистрировано: создаётся при входе
			.EnterNextFromQueue();
	}

	private void OnDestroy()
		=> _container.Dispose();
}
```

### Сохранение модели игрока

`LoadPlayerModelState` регистрирует `IPlayerModelSaveTrigger`, `IPlayerModelContainer<PlayerModel>` и загруженную `PlayerModel`:

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

Чтобы хранить модель в другом месте, реализуйте `IModelLoader<TModel>` и укажите его вторым типом-аргументом `LoadPlayerModelState`.

## Известные ограничения

- `EditorModelLoader.Load` и `YandexModelLoader.Load` вызывают колбэк с загруженной моделью, а затем ещё раз — с новой пустой моделью, поэтому сохранённая модель заменяется пустой. Пока это не исправлено, используйте собственный `IModelLoader<TModel>`, если сохранения должны восстанавливаться.
- `BaseInitializeCheatState.Enter` создаёт префаб консоли, но не вызывает абстрактный метод `AttachCheats()`.
