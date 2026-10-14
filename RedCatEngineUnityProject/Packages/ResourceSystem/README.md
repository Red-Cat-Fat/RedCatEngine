**English** | [Русский](README.ru.md)

# Red Cat Engine: Resources

A thin service over Unity Addressables: load assets by address or `AssetReference` with caching, instantiate prefabs and release everything at once.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.resources` |
| Version | 1.0.1 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `Resources` |
| Namespace | `RedCatEngine.Resources` |

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/ResourceSystem
```

## Dependencies

| Package | Version | Notes |
|---|---|---|
| `com.unity.addressables` | 1.21.17 | Unity registry package, installed automatically |

## API

`IResourceService` (implemented by `AddressableResourceService`):

| Member | Description |
|---|---|
| `Initialize()` | Starts Addressables initialization |
| `Task<T> Load<T>(string address)` | Loads an asset by address; repeated calls return the cached asset |
| `Task<T> Load<T>(AssetReference reference)` | Same, by `AssetReference` (cached by asset GUID) |
| `Task<GameObject> Instantiate(string address)` | Instantiates a prefab; overloads take a position or a parent transform |
| `Cleanup()` | Releases every handle created by `Load` and clears the cache |

## Usage

```csharp
using System.Threading.Tasks;
using RedCatEngine.Resources;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LevelLoader
{
	private readonly IResourceService _resources;

	public LevelLoader(IResourceService resources)
		=> _resources = resources;

	public async Task LoadAsync(AssetReference levelConfigReference, Transform root)
	{
		var config = await _resources.Load<ScriptableObject>(levelConfigReference);
		var hud = await _resources.Instantiate("UI/Hud", root);
	}

	public void Unload()
		=> _resources.Cleanup();
}
```

```csharp
IResourceService resources = new AddressableResourceService();
resources.Initialize();
```

With [Dependency Injection](../DependencyInjection/README.md) the service is bound once and requested through `IResourceService`:

```csharp
container.BindAsSingle<IResourceService>(new AddressableResourceService()).Initialize();
```

`Cleanup()` releases loaded assets only. Objects created with `Instantiate` are regular scene objects; release them with `Addressables.ReleaseInstance` or destroy them together with the scene.
