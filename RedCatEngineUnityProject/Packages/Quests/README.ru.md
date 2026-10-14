[English](README.md) | **Русский**

# Red Cat Engine: Quests

Каркас для квестов: квесты описываются конфигами, создаются фабриками, отслеживаются системами квестов (сюжетные, ежедневные, достижения) и умеют сохраняться и восстанавливаться.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.quests` |
| Версия | 1.1.0 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `Quests` |
| Пространство имён | `RedCatEngine.Quests` |

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Quests
```

## Зависимости

| Пакет | Версия | Для чего |
|---|---|---|
| [Conditions](../Conditions/README.ru.md) | 1.1.0 | `ConditionalQuestConfig` |
| [Configs](../Configs/README.ru.md) | 1.1.0 | конфиги квестов, `ConfigID<QuestConfig>` |
| [Dependency Injection](../DependencyInjection/README.ru.md) | 1.2.0 | квесты создаются с контейнером приложения |
| [Rewards](../Rewards/README.ru.md) | 1.0.1 | награды за квесты |
| [Values](../Values/README.ru.md) | 1.1.0 | поля-значения в конфигах квестов |
| [Serialize Reference Editor](https://github.com/elmortem/serializereferenceeditor) | git | выбор типа `[SR]` — добавляется в проект вручную |

Пакету Values дополнительно нужны [Common services](../UniversalServices/README.ru.md) и [Pools](../Pools/README.ru.md).

## Основные понятия

- **`QuestConfig`** — абстрактный конфиг, который создаёт квест в `DoMake(IApplicationContainer)`.
- **`IQuest`** — запущенный квест: состояние (`QuestState`: `InProgress`, `Complete`, `Finished`, `Fail`, `Skip`), прогресс, локализованные тексты, события `ChangeQuestStateEvent` / `ChangeProgressEvent`, данные для сохранения.
- **Базовые классы квестов** — `BaseQuest`; `BaseCollectProgressQuest` (набрать целевое значение); `BaseDeltaChangeProgressQuest` (изменить значение на заданную величину от стартового). В двух последних прогресс и сохранение уже реализованы.
- **Системы квестов** — владеют списком активных квестов и вызывают `NewQuestEvent`, `ChangeStateQuest`, `QuestsReloaded`:
  - `StoryQuestSystem` — квесты запускаются и завершаются явно;
  - `DailyQuestSystem` — держит `QuestPerDay` случайных квестов из коллекции, каждый живёт `DailyTimeLiveSeconds`;
  - `AchievementQuestSystem` — запускает квесты коллекции.
- **Селекторы и фабрики** — `RandomQuestSelector` / `QueueQuestSelector` выбирают следующий конфиг из коллекции; `CollectionSelectorQuestFactory` и `StoryQuestFactory` создают и восстанавливают квесты.
- **Группирующие конфиги** — `SimpleQuestCollectionConfig` (список квестов), `ConditionalQuestConfig` (первый квест, условие которого истинно), `GroupQuestConfig<T>`, `AllQuestLinksConfig` (все квесты проекта; нужен для восстановления сюжетных квестов).

Ассеты создаются из меню **Configs/Quests/...**.

## Использование

### Квест и его конфиг

```csharp
using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using UnityEngine;

public class KillEnemiesQuest : BaseCollectProgressQuest
{
	private readonly IEnemyEvents _enemies;

	public KillEnemiesQuest(ConfigID<QuestConfig> config, double target, IEnemyEvents enemies)
		: base(config, target)
		=> _enemies = enemies;

	public override string GetLocalizedName() => "Hunter";
	public override string GetLocalizedDescription() => $"Kill {TargetValue} enemies";

	protected override void DoStart() => _enemies.Killed += OnKilled;   // начать слушать события
	protected override void DoReset() => _enemies.Killed -= OnKilled;   // перестать слушать
	protected override void DoResetValue() => SetCurrentValue(0);
	protected override void DoSuccessFinished() { }

	private void OnKilled() => IncrementCurrentValue(); // обновляет прогресс и завершает квест
}

[CreateAssetMenu(menuName = "Configs/Quests/Kill enemies")]
public class KillEnemiesQuestConfig : QuestConfig
{
	[SerializeField] private int _count = 10;

	protected override IQuest DoMake(IApplicationContainer container)
		=> new KillEnemiesQuest(this, _count, container.GetSingle<IEnemyEvents>());
}
```

Когда цель достигнута, квест переходит в `Complete`; вызовите `SuccessFinished()` (например, когда игрок забирает награду), чтобы перевести его в `Finished`.

### Минимальная настройка сюжетных квестов

Короткая обучающая цепочка: сначала *Поговорить с кузнецом*, затем *Убить 5 волков*. Вся настройка — один класс квеста, один класс конфига, несколько ассетов и небольшой «режиссёр», который двигает сюжет вперёд.

#### 1. Игровые события

Квестам нужно узнавать, что происходит в игре. Минимальный способ — один сервис с одним событием:

```csharp
using System;

public interface IStoryEvents
{
	event Action<string> Happened;
	void Raise(string eventId);
}

public class StoryEvents : IStoryEvents
{
	public event Action<string> Happened;

	public void Raise(string eventId) => Happened?.Invoke(eventId);
}
```

Игровой код сообщает о событиях по идентификатору: `storyEvents.Raise("talk_blacksmith")`, `storyEvents.Raise("wolf_killed")`.

#### 2. Один класс квеста на много квестов

Квест-счётчик — «событие произошло N раз» — покрывает большую часть типичного сюжета. В `BaseCollectProgressQuest` прогресс, завершение и сохранение уже реализованы:

```csharp
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;

public class CounterQuest : BaseCollectProgressQuest
{
	private readonly IStoryEvents _events;
	private readonly string _eventId;
	private readonly string _title;

	public CounterQuest(ConfigID<QuestConfig> config, string title, string eventId, int count, IStoryEvents events)
		: base(config, count)
	{
		_title = title;
		_eventId = eventId;
		_events = events;
	}

	public override string GetLocalizedName() => _title;
	public override string GetLocalizedDescription() => $"{_title}: {GetProcessProgressText()}";

	protected override void DoStart() => _events.Happened += OnHappened;
	protected override void DoReset() => _events.Happened -= OnHappened;
	protected override void DoResetValue() => SetCurrentValue(0);
	protected override void DoSuccessFinished() { }

	private void OnHappened(string eventId)
	{
		if (eventId == _eventId)
			IncrementCurrentValue();
	}
}
```

#### 3. Конфиг

```csharp
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Quests/Counter quest")]
public class CounterQuestConfig : QuestConfig
{
	[SerializeField] private string _title;
	[SerializeField] private string _eventId;
	[SerializeField] private int _count = 1;

	protected override IQuest DoMake(IApplicationContainer container)
		=> new CounterQuest(this, _title, _eventId, _count, container.GetSingle<IStoryEvents>());
}
```

#### 4. Ассеты

1. **Configs/Quests/Counter quest** → `TalkToBlacksmith`: название *Поговорить с кузнецом*, событие `talk_blacksmith`, количество `1`.
2. **Configs/Quests/Counter quest** → `KillWolves`: название *Убить волков*, событие `wolf_killed`, количество `5`.
3. **Configs/Quests/Quest Systems/AllQuestCollection** → `AllQuests`: добавьте оба квеста в *All Configs* (или вызовите `CollectLinks()` из редакторного скрипта).

`AllQuestLinksConfig` — список, из которого фабрика сюжетных квестов создаёт квесты: квест, которого в нём нет, нельзя ни запустить (`NotFoundQuestException`), ни восстановить из сохранения.

#### 5. Регистрация системы

```csharp
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.QuestSystems;

container.BindType<IStoryEvents, StoryEvents>();
container.BindAsSingle(allQuests);                  // ассет AllQuests
container.BindType<StoryQuestFactory>(container);   // создаёт квесты с контейнером
var story = container.BindType<StoryQuestSystem>();

story.LoadData(QuestsDataContainer.Empty);          // или сохранённые данные, см. шаг 7
```

#### 6. Продвижение сюжета

Система только выполняет квесты; какой квест идёт за каким, решает ваш код. Небольшой режиссёр реагирует на смену состояний:

```csharp
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.QuestSystems;
using UnityEngine;

public class TutorialDirector
{
	private readonly StoryQuestSystem _story;
	private readonly CounterQuestConfig _talk;
	private readonly CounterQuestConfig _wolves;

	public TutorialDirector(StoryQuestSystem story, CounterQuestConfig talk, CounterQuestConfig wolves)
	{
		_story = story;
		_talk = talk;
		_wolves = wolves;

		_story.NewQuestEvent += quest => Debug.Log($"New quest: {quest.GetLocalizedName()}");
		_story.ChangeStateQuest += OnQuestStateChanged;
	}

	public void Begin()
	{
		if (!IsKnown(_talk))
			_story.StartQuest(_talk);
	}

	private void OnQuestStateChanged(IQuest quest)
	{
		if (quest.QuestState != QuestState.Complete)
			return;

		if (quest.Config == _talk)
		{
			_story.FinishedQuest(_talk);           // Complete → Finished
			if (!IsKnown(_wolves))
				_story.StartQuest(_wolves);        // следующий шаг сюжета
		}
		else if (quest.Config == _wolves)
		{
			_story.FinishedQuest(_wolves);
		}
	}

	// уже запускался: активен, выполнен или завершён
	private bool IsKnown(QuestConfig config)
		=> _story.GetActiveQuests().Exists(quest => quest.Config == config);
}
```

```csharp
var director = new TutorialDirector(story, talkToBlacksmithConfig, killWolvesConfig);
director.Begin();

storyEvents.Raise("talk_blacksmith"); // Talk → Complete → Finished, начинается «Убить волков»
storyEvents.Raise("wolf_killed");     // прогресс 1/5
```

Квест переходит из `InProgress` в `Complete`, когда цель достигнута. `FinishedQuest` переводит его в `Finished` — вызывайте сразу, как здесь, или когда игрок забирает награду.

#### 7. Сохранение и восстановление

Состояние сюжета — это `story.GetData()`. Чтобы хранить его через [систему сохранений](../UniversalServices/README.ru.md#сохранения) пакета Common services, оберните систему в `ISaved`:

```csharp
using RedCatEngine.CommonServices.Services.Save.Realisations;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.QuestSystems;

public class StoryQuestsSave : ISaved
{
	private readonly StoryQuestSystem _story;
	private readonly SaveSerializer<QuestsDataContainer> _serializer = new();

	public StoryQuestsSave(StoryQuestSystem story) => _story = story;

	public string SaveKey => "StoryQuests";

	public string GetSerializedSaveData() => _serializer.SaveDataFrom(_story.GetData());

	public void Load(string json) => _story.LoadData(_serializer.LoadDataFrom(json));
}
```

Без системы сохранений то же самое делают `JsonUtility.ToJson(story.GetData())` и `story.LoadData(JsonUtility.FromJson<QuestsDataContainer>(json))`. При загрузке каждый квест пересоздаётся через `AllQuestLinksConfig`, получает обратно своё состояние и прогресс и снова слушает игру.

Восстанавливайте сохранение **до** `director.Begin()`, чтобы режиссёр видел, какие квесты уже запускались.

#### Что важно знать

- `StartQuest` не проверяет, запущен ли квест; двойной вызов создаёт две копии. Сначала проверяйте `GetActiveQuests()`, как делает `IsKnown` выше.
- Завершённые квесты остаются в `GetActiveQuests()` и в сохранении с состоянием `Finished`. Так сюжет помнит, что уже сделано; при показе журнала квестов фильтруйте по `QuestState`.
- Сохранять можно только квесты, унаследованные от `BaseCollectProgressQuest` или `BaseDeltaChangeProgressQuest`: `QuestsDataContainer` хранит только их типы данных и бросает исключение для остальных.

### Ежедневные квесты и достижения

Обе системы создаются из своих ассетов-конфигов:

```csharp
DailyQuestSystem daily = dailyQuestSystemConfig.Make(container);
AchievementQuestSystem achievements = achievementQuestSystemConfig.Make(container);

daily.NewQuestEvent += quest => { /* показать новый квест */ };
daily.LoadData(savedData ?? QuestsDataContainer.Empty);

foreach (var quest in daily.GetActiveQuests())
	Debug.Log($"{quest.GetLocalizedName()} {quest.GetProcessProgressText()}");
```

### Сохранение и восстановление

```csharp
QuestsDataContainer data = daily.GetData();   // сериализуемый снимок активных квестов
// ... сохранить, а позже:
daily.LoadData(data);                         // пересоздаёт квесты и продолжает их
```

`LoadData(QuestsDataContainer.Empty)` запускает систему без сохранённых квестов. Вызывайте `Dispose` у системы квестов, когда она больше не нужна.

## Примечания

- Системы квестов берут текущее время из `DateTime.UtcNow`.
