**English** | [Русский](README.ru.md)

# Red Cat Engine: Quests

A quest framework: quests are described by configs, created by factories, tracked by quest systems (story, daily, achievements) and can be saved and restored.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.quests` |
| Version | 1.1.0 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `Quests` |
| Namespace | `RedCatEngine.Quests` |

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Quests
```

## Dependencies

| Package | Version | Used for |
|---|---|---|
| [Conditions](../Conditions/README.md) | 1.1.0 | `ConditionalQuestConfig` |
| [Configs](../Configs/README.md) | 1.1.0 | quest configs, `ConfigID<QuestConfig>` |
| [Dependency Injection](../DependencyInjection/README.md) | 1.2.0 | quests are created with the application container |
| [Rewards](../Rewards/README.md) | 1.0.1 | quest rewards |
| [Values](../Values/README.md) | 1.1.0 | value fields in quest configs |
| [Serialize Reference Editor](https://github.com/elmortem/serializereferenceeditor) | git | the `[SR]` type picker — add it to the project manually |

Values additionally requires [Common services](../UniversalServices/README.md) and [Pools](../Pools/README.md).

## Concepts

- **`QuestConfig`** — an abstract config that creates a quest in `DoMake(IApplicationContainer)`.
- **`IQuest`** — a running quest: state (`QuestState`: `InProgress`, `Complete`, `Finished`, `Fail`, `Skip`), progress, localized texts, `ChangeQuestStateEvent` / `ChangeProgressEvent`, save data.
- **Quest base classes** — `BaseQuest`; `BaseCollectProgressQuest` (reach a target value); `BaseDeltaChangeProgressQuest` (change a value by a delta from the start value). The last two already implement progress and saving.
- **Quest systems** — own the list of active quests and raise `NewQuestEvent`, `ChangeStateQuest`, `QuestsReloaded`:
  - `StoryQuestSystem` — quests are started and finished explicitly;
  - `DailyQuestSystem` — keeps `QuestPerDay` random quests from a collection, each living `DailyTimeLiveSeconds`;
  - `AchievementQuestSystem` — runs the quests of a collection.
- **Selectors and factories** — `RandomQuestSelector` / `QueueQuestSelector` choose the next config from a collection; `CollectionSelectorQuestFactory` and `StoryQuestFactory` create and restore quests.
- **Grouping configs** — `SimpleQuestCollectionConfig` (a list of quests), `ConditionalQuestConfig` (the first quest whose condition is true), `GroupQuestConfig<T>`, `AllQuestLinksConfig` (every quest in the project, required to restore story quests).

Assets are created from **Configs/Quests/...**.

## Usage

### A quest and its config

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

	protected override void DoStart() => _enemies.Killed += OnKilled;   // start listening
	protected override void DoReset() => _enemies.Killed -= OnKilled;   // stop listening
	protected override void DoResetValue() => SetCurrentValue(0);
	protected override void DoSuccessFinished() { }

	private void OnKilled() => IncrementCurrentValue(); // updates progress and completes the quest
}

[CreateAssetMenu(menuName = "Configs/Quests/Kill enemies")]
public class KillEnemiesQuestConfig : QuestConfig
{
	[SerializeField] private int _count = 10;

	protected override IQuest DoMake(IApplicationContainer container)
		=> new KillEnemiesQuest(this, _count, container.GetSingle<IEnemyEvents>());
}
```

When the target is reached the quest becomes `Complete`; call `SuccessFinished()` (for example when the player claims the reward) to move it to `Finished`.

### Minimal story quest setup

A short tutorial chain: first *Talk to the blacksmith*, then *Kill 5 wolves*. The whole setup is one quest class, one config class, a few assets and a small "director" that moves the story forward.

#### 1. Game events

Quests need to learn what happens in the game. A minimal way is one service with a single event:

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

Game code reports events by id: `storyEvents.Raise("talk_blacksmith")`, `storyEvents.Raise("wolf_killed")`.

#### 2. One quest class for many quests

A counter quest — "the event happened N times" — covers a large part of a typical story. `BaseCollectProgressQuest` already implements progress, completion and saving:

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

#### 3. The config

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

#### 4. Assets

1. **Configs/Quests/Counter quest** → `TalkToBlacksmith`: title *Talk to the blacksmith*, event `talk_blacksmith`, count `1`.
2. **Configs/Quests/Counter quest** → `KillWolves`: title *Kill wolves*, event `wolf_killed`, count `5`.
3. **Configs/Quests/Quest Systems/AllQuestCollection** → `AllQuests`: add both quests to *All Configs* (or call `CollectLinks()` from an editor script).

`AllQuestLinksConfig` is the list the story factory creates quests from: a quest that is not in it cannot be started (`NotFoundQuestException`) or restored from a save.

#### 5. Register the system

```csharp
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Data;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.QuestSystems;

container.BindType<IStoryEvents, StoryEvents>();
container.BindAsSingle(allQuests);                  // the AllQuests asset
container.BindType<StoryQuestFactory>(container);   // creates quests with the container
var story = container.BindType<StoryQuestSystem>();

story.LoadData(QuestsDataContainer.Empty);          // or the saved data, see step 7
```

#### 6. Move the story forward

The system only runs quests; which quest follows which is decided by your code. A small director reacts to state changes:

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
				_story.StartQuest(_wolves);        // the next step of the story
		}
		else if (quest.Config == _wolves)
		{
			_story.FinishedQuest(_wolves);
		}
	}

	// started before: active, complete or finished
	private bool IsKnown(QuestConfig config)
		=> _story.GetActiveQuests().Exists(quest => quest.Config == config);
}
```

```csharp
var director = new TutorialDirector(story, talkToBlacksmithConfig, killWolvesConfig);
director.Begin();

storyEvents.Raise("talk_blacksmith"); // Talk → Complete → Finished, Kill wolves starts
storyEvents.Raise("wolf_killed");     // progress 1/5
```

A quest goes `InProgress` → `Complete` when its target is reached. `FinishedQuest` moves it to `Finished` — call it right away, as here, or when the player claims a reward.

#### 7. Save and restore

The state of the story is `story.GetData()`. To store it with the [save system](../UniversalServices/README.md#save) of Common services, wrap the system in an `ISaved`:

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

Without the save system, `JsonUtility.ToJson(story.GetData())` and `story.LoadData(JsonUtility.FromJson<QuestsDataContainer>(json))` do the same. On load every quest is recreated through `AllQuestLinksConfig`, gets its state and progress back and continues listening to the game.

Restore the save **before** `director.Begin()`, so the director sees which quests were already started.

#### Things to know

- `StartQuest` does not check whether the quest is already running; calling it twice creates two copies. Check `GetActiveQuests()` first, as `IsKnown` does above.
- Finished quests stay in `GetActiveQuests()` and in the save with the `Finished` state. This is how the story remembers what has been done; filter by `QuestState` when showing the quest log.
- Only quests derived from `BaseCollectProgressQuest` or `BaseDeltaChangeProgressQuest` can be saved: `QuestsDataContainer` stores only their data types and throws for any other.

### Daily quests and achievements

Both systems are created from their config assets:

```csharp
DailyQuestSystem daily = dailyQuestSystemConfig.Make(container);
AchievementQuestSystem achievements = achievementQuestSystemConfig.Make(container);

daily.NewQuestEvent += quest => { /* show a new quest */ };
daily.LoadData(savedData ?? QuestsDataContainer.Empty);

foreach (var quest in daily.GetActiveQuests())
	Debug.Log($"{quest.GetLocalizedName()} {quest.GetProcessProgressText()}");
```

### Save and restore

```csharp
QuestsDataContainer data = daily.GetData();   // serializable snapshot of the active quests
// ... store it, later:
daily.LoadData(data);                         // recreates the quests and continues them
```

`LoadData(QuestsDataContainer.Empty)` starts the system without saved quests. Dispose a quest system when it is no longer needed.

## Notes

- Quest systems take the current time from `DateTime.UtcNow`.
