# RedCatEngine — AGENTS.md

Single source of instructions for AI coding agents in this repository. `CLAUDE.md` imports this file; edit project rules here, not there.

## What this repository is

RedCatEngine is a set of independent Unity UPM packages for game development. Work here means writing and extending the packages; consumers never clone the repo — they pull individual packages by git URL:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/<PackageFolder>
```

Consequence: every package must compile and make sense on its own, with only the packages it references. `RedCatEngineUnityProject/` is just the host Unity project used to develop and test them (the repo root is **not** the Unity project root).

## Code access policy: graphify first (mandatory)

All code lookups go through the graphify knowledge graph in `graphify-out/` — do not start with grep, glob or file reads over `Packages/`.

1. Locate and understand code with the graph:
   - `graphify query "<question>"` — scoped subgraph for a question (raise `--budget N` if the output says TRUNCATED)
   - `graphify explain "<Symbol>"` — a node and its neighbours
   - `graphify path "<A>" "<B>"` — how two symbols are connected
   - `graphify affected "<Symbol>"` — reverse dependencies; run this **before changing any public type or interface**, because other packages and external consumers depend on them
   - `graphify god-nodes` — architectural hubs
2. Only then read the specific files/lines the graph pointed to (results carry `src=` and `loc=`).
3. Raw text/file search is a fallback for what the graph does not index: `.asmdef`, `package.json`, `.meta`, YAML assets, string literals. Say so when you fall back.
4. After any code change run `graphify update .` (AST-only, no LLM, no API cost) so the graph is never stale. After deleting/renaming code use `graphify update . --force`.

`.graphifyignore` keeps `Library/`, `ProjectSettings/`, `.meta`, `.csproj` out of the graph. If `graphify-out/graph.json` is missing, build it with `graphify update .` before doing anything else.

## Commands

There is no build, lint, or format step — Unity compiles the packages; `.csproj`/`.sln` are generated and gitignored. Source of truth for assemblies is `.asmdef` + `package.json`.

Tests are EditMode NUnit tests (Unity Test Framework) in `Packages/<Name>/Tests/`. Run them from the Unity Test Runner window, or headless (the editor must not have the project open; `<Unity>` is the 6000.0.41f1 editor executable):

```bash
# all EditMode tests
<Unity> -batchmode -projectPath RedCatEngineUnityProject -runTests -testPlatform EditMode -testResults results.xml -logFile -

# one package's tests (assembly is <PackageFolder>Tests, e.g. QuestsTests, WindowsTests, UniversalServicesTests)
<Unity> -batchmode -projectPath RedCatEngineUnityProject -runTests -testPlatform EditMode -assemblyNames StateMachineTests -testResults results.xml -logFile -

# one test / one fixture
<Unity> -batchmode -projectPath RedCatEngineUnityProject -runTests -testPlatform EditMode -testFilter "RedCatEngine.StateMachine.Tests.EnterExitStateTests.GivenStateMachine_WhenEnterToSimpleState_ThenSwitched" -testResults results.xml -logFile -
```

CI: one workflow per package, `.github/workflows/tests-<package>.yml`, each run on every push. They open the whole host project with `game-ci/unity-test-runner` and select the package with `-assemblyNames <PackageFolder>Tests`. CI uses Unity **2021.3.31f1**, while the local editor is **6000.0.41f1** — avoid APIs newer than 2021.3 in packages that declare `"unity": "2021.3"` (only GameSettings declares `6000.0`). A new package needs its own workflow file (copy an existing one) and a `Tests/<PackageFolder>Tests.asmdef`.

## Architecture

### Package dependencies are declared in three places that must stay in sync

1. `.asmdef` `references` (by GUID) — what actually compiles.
2. `package.json` `dependencies` — RedCat package id + the **current version** of that package. Bumping a package's version means updating the `dependencies` entry in every package that depends on it.
3. The "Dependencies between packages" table in the root `README.md`.

```
DependencyInjection, StateMachine, Configs, Pools, Benchmark   — no dependencies
Resources (folder ResourceSystem)   → com.unity.addressables
CommonServices (folder UniversalServices) → DependencyInjection, Configs, Pools
Values        → DependencyInjection, Configs, CommonServices, SREditor*
MvpWindows (folder Windows) → DependencyInjection, Configs, CommonServices
GameSettings  → Configs, SREditor*
Conditions    → DependencyInjection, Configs, Values, SREditor*
Rewards       → DependencyInjection, Configs, Values, Conditions, SREditor*
Quests        → DependencyInjection, Configs, Values, Conditions, Rewards, SREditor*
ApplicationRunner → DependencyInjection, StateMachine, LocalizeSystem*
```

`*` `SREditor` = `com.elmortem.serializereferenceeditor`, `LocalizeSystem` = `com.elmortem.localization`. These are git packages, which UPM cannot express in `package.json` `dependencies` — they are referenced only in the `.asmdef`, and consumers add them manually (documented in README). Do not add them to `package.json`.

Consumers install each required RedCat package by its own git URL. Keep the graph acyclic; a type needed by two leaf packages belongs in a lower one. `DoTweenExtensions` in UniversalServices compiles only under the `DOTWEEN` scripting define.

### Names that do not match

- Folder ≠ assembly: `ResourceSystem/` → assembly `Resources`; `UniversalServices/` → assembly `CommonServices`, namespace `RedCatEngine.CommonServices`; `Windows/` → assembly `MvpWindows`.
- **Never name an assembly `Windows`**: Unity's player build treats that name as the WinRT metadata assembly and silently drops serialized fields of its types, corrupting the built player (see `Windows/CHANGELOG.md` 2.0.1). Renames must keep the asmdef GUID.
- Root namespace is `RedCatEngine.<Assembly>`, except the `Windows` package runtime code, which uses `Infrastructure.Windows.*`.
- Published package ids contain typos that are part of the public install contract — do not "fix" them casually: `...red-cat-engine.aplication.runner`, `...red-cat-engine.banchmark`.
- Test assemblies are `<PackageFolder>Tests`; only the asmdef *file* name differs in DI/StateMachine (`Tests.asmdef`).

### DependencyInjection is the spine

`ApplicationContainer` (pure C#) and `UnityGameContainer` (adds prefab instantiation) are a reflection-based container with named parent/child scopes; lookups fall through to the parent.

- **Scopes**: `CreateChildContainer(name, ContainerModifyRule)` — the rule (`UnityContainerModifyRule` for the Unity container) is a composable list of bind actions applied when the child is built; it also binds the container itself as `ICreator` and the binder/getter interfaces. Children share the parent's `ChildTypesCache`, so the assembly scan happens once. `IContainerDiagnostics` traces container activity, and `DependencyInjection/Editor/ContainerHierarchyWindow` shows the live container tree.
- **Binding**: `BindAsSingle` / `BindAsArray` for instances, `BindType<TBind, TImpl>` to construct-and-bind. Binding a second instance of a type without the array API throws `BindDuplicateWithoutArrayMarkException`; `ReBindAsSingle` replaces.
- **Construction**: `Create<T>(context)` picks the constructor marked `[Inject]`, falls back to a parameterless one, otherwise throws. So every service with dependencies needs `[Inject]` on its constructor.
- **Resolution order** for each parameter: the `params object[] context` passed to the call (matched by type, nested arrays included) → exact type in the container → any bound instance assignable to the type → parent container → **auto-create and bind as single** (concrete non-`UnityEngine.Object` types only; otherwise `NotFoundInstanceOrCreateException`). `context` is how per-call data (configs, models, payloads) reaches constructors without being bound.
- **Deferred dependencies**: a constructor parameter of type `ISingleProvider<T>` / `IArrayProvider<T>` is satisfied immediately with a provider that is filled when `T` is bound later — use it to break ordering/cyclic dependencies.
- **MonoBehaviours**: derive from `MonoConstruct` and expose a public method marked `[MonoInject]` instead of a constructor (`OnValidate` logs an error if it is missing); the container calls it, then `FinishInitialize()` → `DoInitialize()`. Cleanup goes in `DoDisposable()`, triggered from `OnDisable`.

### How the other systems plug into DI

- **StateMachine**: `BaseTypedStateMachine<TBaseState>` keys states by `Type`, supports `IState` and `IPayloadedState<TPayload>`, plus a queue (`AddToQueue` / `EnterNextFromQueue` / `ClearQueue`) and step history. It has no DI dependency.
- **ApplicationRunner**: `TypeBasedStateMachine` bridges the two — states are created through `ICreator`, and unknown states are created lazily on first `Enter`. It also contains boot states (localization, cheats, player model load/save) with Editor and Yandex variants.
- **Windows** (MVP): a window is described by a `WindowConfig` ScriptableObject that names its model type; `WindowService.Open(config, context)` creates the model through the injected `IUnityGameContainer`, passing `context` + the config, and manages parent/child window hierarchy, layers, and hide/show of all windows.
- **Configs**: `BaseConfig` is a ScriptableObject whose `ID` is derived from the asset GUID in `OnValidate`, and equality is by that ID. Values/Conditions/Rewards/Quests are data-driven on top of it, using `[SerializeReference]` polymorphic fields (hence the SREditor dependency).

## Conventions

- Tabs for indentation, private fields `_camelCase`, Allman braces.
- Tests: `Given..._When..._Then...` names; test doubles live in `Tests/SpecialSubClasses/`.
- Every new file/folder under `Packages/` needs its Unity `.meta` committed alongside (let the editor generate it; never hand-write GUIDs).
- Changing a package's behaviour: bump `version` in its `package.json` (SemVer; mark breaking changes `BREAKING:`), add a `CHANGELOG.md` entry in Keep a Changelog format, and update the `dependencies` version in dependant packages.

## Documentation

Documentation is bilingual: **English is the primary language, Russian is the secondary translation**. Write the English version first and keep the Russian one in sync in the same change; never leave the Russian file ahead of or contradicting the English one. Code, identifiers, comments, commit messages stay in English.
