# RedCatEngine

This is an extension package that represents a set of different functionality for creating a game. The main focus in version 1.0.0 is on Unity, but it can be adapted for other game engines. 
Individual packages may have dependencies on other packages that are hosted in a separate repository, namely:
- [Serialize Reference Editor](https://github.com/elmortem/serializereferenceeditor "For working with Serialize reference")
- [Localization](https://github.com/elmortem/localization "To implement a localization system")

## Pools

Special pool system.

Serializable prefab spawner^
```cs
		[SerializeField]
		private UnityComponentPoolSerializable<*BasePooledObject*> _items;
```

## Configs

Base configs

### Installation Configs

1. Open Package Manager from Window > Package Manager
2. Click on the "+" button > Add package from git URL
3. Enter the following URL

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Configs
```

## Quests

Base quests

### Installation Quests

1. Open Package Manager from Window > Package Manager
2. Click on the "+" button > Add package from git URL
3. Enter the following URL

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Quests
```

## Resources

This is a package that provides a basic system for work with Addresables

### Installation Resources

1. Open Package Manager from Window > Package Manager
2. Click on the "+" button > Add package from git URL
3. Enter the following URL

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/ResourceSystem
```

## Dependency injection (DI)

Simple service locator

### Installation DI

1. Open Package Manager from Window > Package Manager
2. Click on the "+" button > Add package from git URL
3. Enter the following URL

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/DependencyInjection
```

## State machine

This is a package that provides a basic State machine for implementing the states of a game project.

### Installation State machine

1. Open Package Manager from Window > Package Manager
2. Click on the "+" button > Add package from git URL
3. Enter the following URL

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/StateMachine
```

## Windows

Simple window system, based on MVP pattern.

### Install Windows

1. Open Package Manager from Window > Package Manager
2. Click on the "+" button > Add package from git URL
3. Enter the following URL

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Windows
```


## Dependencies between packages

Packages declare their dependencies on each other in `package.json`. Install all required packages by git URL (see the sections above); versions are taken from `package.json` of each package.

| Package | Requires |
|---|---|
| ApplicationRunner | DependencyInjection, StateMachine |
| Conditions | Configs, DependencyInjection, Values |
| GameSettings | Configs |
| Quests | Conditions, Configs, DependencyInjection, Rewards, Values |
| ResourceSystem | `com.unity.addressables` |
| Rewards | Conditions, Configs, DependencyInjection, Values |
| UniversalServices | Configs, DependencyInjection, Pools |
| Values | Configs, DependencyInjection, UniversalServices |
| Windows | Configs, DependencyInjection, UniversalServices |

Git packages cannot be declared as UPM dependencies, so add them to the project manually:

- `com.elmortem.serializereferenceeditor` (Conditions, Values, Quests, Rewards, GameSettings)
- `com.elmortem.localization` (ApplicationRunner)

`DoTweenExtensions` from UniversalServices is compiled only when the `DOTWEEN` scripting define is set (DOTween installed).
