[English](README.md) | **Русский**

# Red Cat Engine: Resources

Тонкий сервис поверх Unity Addressables: загрузка ассетов по адресу или `AssetReference` с кешированием, создание префабов и освобождение всего загруженного одним вызовом.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.resources` |
| Версия | 1.0.1 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `Resources` |
| Пространство имён | `RedCatEngine.Resources` |

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/ResourceSystem
```

## Зависимости

| Пакет | Версия | Примечание |
|---|---|---|
| `com.unity.addressables` | 1.21.17 | пакет из реестра Unity, устанавливается автоматически |

## API

`IResourceService` (реализация — `AddressableResourceService`):

| Член | Описание |
|---|---|
| `Initialize()` | Запускает инициализацию Addressables |
| `Task<T> Load<T>(string address)` | Загружает ассет по адресу; повторные вызовы возвращают ассет из кеша |
| `Task<T> Load<T>(AssetReference reference)` | То же по `AssetReference` (кеш по GUID ассета) |
| `Task<GameObject> Instantiate(string address)` | Создаёт экземпляр префаба; есть перегрузки с позицией и с родительским трансформом |
| `Cleanup()` | Освобождает все хендлы, созданные через `Load`, и очищает кеш |

## Использование

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

С пакетом [Dependency Injection](../DependencyInjection/README.ru.md) сервис регистрируется один раз и запрашивается через `IResourceService`:

```csharp
container.BindAsSingle<IResourceService>(new AddressableResourceService()).Initialize();
```

`Cleanup()` освобождает только загруженные ассеты. Объекты, созданные через `Instantiate`, — обычные объекты сцены; освобождайте их через `Addressables.ReleaseInstance` или уничтожайте вместе со сценой.
