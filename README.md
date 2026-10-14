**English** | [Русский](README.ru.md)

# RedCatEngine

RedCatEngine is a set of Unity packages that cover the common parts of a game: dependency injection, application bootstrap, state machines, UI windows, pools, configs, data-driven values, conditions, rewards and quests, services and tools.

The packages are independent. You do not clone this repository into your game — you add only the packages you need through the Unity Package Manager.

## Packages

| Package | What it is for | Version | Documentation |
|---|---|---|---|
| Dependency Injection | DI container: bindings, `[Inject]` constructors, child scopes, `MonoBehaviour` injection | 1.2.1 | [EN](RedCatEngineUnityProject/Packages/DependencyInjection/README.md) · [RU](RedCatEngineUnityProject/Packages/DependencyInjection/README.ru.md) |
| State Machine | Typed state machine with payload states and a state queue | 2.0.1 | [EN](RedCatEngineUnityProject/Packages/StateMachine/README.md) · [RU](RedCatEngineUnityProject/Packages/StateMachine/README.ru.md) |
| Application runner | Game bootstrap: DI-created states, localization, player model loading and saving | 1.1.1 | [EN](RedCatEngineUnityProject/Packages/ApplicationRunner/README.md) · [RU](RedCatEngineUnityProject/Packages/ApplicationRunner/README.ru.md) |
| Configs | `ScriptableObject` configs with stable ids and typed id references | 1.1.1 | [EN](RedCatEngineUnityProject/Packages/Configs/README.md) · [RU](RedCatEngineUnityProject/Packages/Configs/README.ru.md) |
| Pools | Object pools for prefabs, configured in the Inspector or from code | 1.1.0 | [EN](RedCatEngineUnityProject/Packages/Pools/README.md) · [RU](RedCatEngineUnityProject/Packages/Pools/README.ru.md) |
| Common services | Logs, time, update loop, callbacks, random, save, tags, sound, storages | 2.0.1 | [EN](RedCatEngineUnityProject/Packages/UniversalServices/README.md) · [RU](RedCatEngineUnityProject/Packages/UniversalServices/README.ru.md) |
| Windows | MVP window system with layers and parent windows | 2.0.2 | [EN](RedCatEngineUnityProject/Packages/Windows/README.md) · [RU](RedCatEngineUnityProject/Packages/Windows/README.ru.md) |
| Values | Numbers and flags assembled in the Inspector from constants, variables, curves and operations | 1.1.1 | [EN](RedCatEngineUnityProject/Packages/Values/README.md) · [RU](RedCatEngineUnityProject/Packages/Values/README.ru.md) |
| Conditions | Checks assembled in the Inspector from logic blocks and your own service checks | 1.1.1 | [EN](RedCatEngineUnityProject/Packages/Conditions/README.md) · [RU](RedCatEngineUnityProject/Packages/Conditions/README.ru.md) |
| Rewards | Rewards described in configs and applied by one service | 1.0.2 | [EN](RedCatEngineUnityProject/Packages/Rewards/README.md) · [RU](RedCatEngineUnityProject/Packages/Rewards/README.ru.md) |
| Quests | Quest configs, story / daily / achievement quest systems, saving | 1.2.0 | [EN](RedCatEngineUnityProject/Packages/Quests/README.md) · [RU](RedCatEngineUnityProject/Packages/Quests/README.ru.md) |
| Resources | Addressables wrapper: cached loading, instantiation, cleanup | 1.0.1 | [EN](RedCatEngineUnityProject/Packages/ResourceSystem/README.md) · [RU](RedCatEngineUnityProject/Packages/ResourceSystem/README.ru.md) |
| Game Settings | Graphics settings as data with saved player overrides | 2.0.1 | [EN](RedCatEngineUnityProject/Packages/GameSettings/README.md) · [RU](RedCatEngineUnityProject/Packages/GameSettings/README.ru.md) |
| Benchmark | FPS and memory measurement of a gameplay scenario into a report file | 1.1.0 | [EN](RedCatEngineUnityProject/Packages/Benchmark/README.md) · [RU](RedCatEngineUnityProject/Packages/Benchmark/README.ru.md) |

Each package page contains its description, dependencies with links, and usage examples.

## Installation

1. Open **Window > Package Manager**.
2. Click **+** > **Add package from git URL**.
3. Enter the URL of the package:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/<PackageFolder>
```

`<PackageFolder>` is the folder name: `ApplicationRunner`, `Benchmark`, `Conditions`, `Configs`, `DependencyInjection`, `GameSettings`, `Pools`, `Quests`, `ResourceSystem`, `Rewards`, `StateMachine`, `UniversalServices`, `Values`, `Windows`.

Install the dependencies of a package the same way, starting from the packages that have none. The same can be done by editing `Packages/manifest.json` — for example, for Windows:

```json
{
  "dependencies": {
    "com.boronnikov.games.red-cat-engine.configs": "https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Configs",
    "com.boronnikov.games.red-cat-engine.di": "https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/DependencyInjection",
    "com.boronnikov.games.red-cat-engine.pools": "https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Pools",
    "com.boronnikov.games.red-cat-engine.services": "https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/UniversalServices",
    "com.boronnikov.games.red-cat-engine.windows": "https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Windows"
  }
}
```

## Dependencies between packages

Packages declare their dependencies on each other in `package.json`. Install all required packages by git URL; versions are taken from `package.json` of each package.

| Package | Requires |
|---|---|
| DependencyInjection | — |
| StateMachine | — |
| Configs | — |
| Pools | — |
| Benchmark | — |
| ApplicationRunner | DependencyInjection, StateMachine |
| Conditions | Configs, DependencyInjection, Values |
| GameSettings | Configs |
| Quests | Conditions, Configs, DependencyInjection, Rewards, Values |
| ResourceSystem | `com.unity.addressables` |
| Rewards | Conditions, Configs, DependencyInjection, Values |
| UniversalServices | Configs, DependencyInjection, Pools |
| Values | Configs, DependencyInjection, UniversalServices |
| Windows | Configs, DependencyInjection, UniversalServices |

Dependencies are transitive: Quests, for example, also needs everything required by Conditions, Rewards and Values.

Git packages cannot be declared as UPM dependencies, so add them to the project manually:

- [`com.elmortem.serializereferenceeditor`](https://github.com/elmortem/serializereferenceeditor) — Conditions, Values, Quests, Rewards, GameSettings
- [`com.elmortem.localization`](https://github.com/elmortem/localization) — ApplicationRunner

`DoTweenExtensions` from UniversalServices is compiled only when the `DOTWEEN` scripting define is set (DOTween installed).

## Requirements

Unity 2021.3 or newer; Game Settings requires Unity 6000.0 or newer.

## Repository

`RedCatEngineUnityProject/` is the Unity project used to develop and test the packages; the packages themselves are in `RedCatEngineUnityProject/Packages/`. Every package has EditMode tests that run in GitHub Actions on each push.
