[English](README.md) | **Русский**

# Red Cat Engine: Game Settings

Графические настройки как данные: настройки по умолчанию перечислены в ассете-конфиге, изменения игрока хранятся в JSON-файле, и каждая настройка умеет применять сама себя.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.gamesettings` |
| Версия | 2.0.0 ([список изменений](CHANGELOG.md)) |
| Unity | 6000.0+ |
| Сборка | `GameSettings` |
| Пространство имён | `RedCatEngine.GameSettings` |

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/GameSettings
```

## Зависимости

| Пакет | Версия | Для чего |
|---|---|---|
| [Configs](../Configs/README.ru.md) | 1.1.0 | `GameVisualSettingsConfig` |
| [Serialize Reference Editor](https://github.com/elmortem/serializereferenceeditor) | git | выбор типа `[SR]` — добавляется в проект вручную |

## Основные понятия

- **`BaseGameSetting`** — одна настройка: ключ `SaveKey` и метод `Apply()`. Переопределите `DoApply()` и при необходимости `IsCanApply()`.
- **`GameVisualSettingsConfig`** — ассет с настройками по умолчанию (`BaseSettings`), создаётся из меню **Configs/Common/Game Settings Config**.
- **`OverrideSettingsData`** — изменения игрока, сохраняются в `<persistentDataPath>/<проект>_overrideSettingsData.json`.
- **`GameVisualSettingService`** — применяет настройки по умолчанию, затем изменения игрока; добавляет, сохраняет и сбрасывает изменения; вызывает `GameSettingChangeEvent`.
- **`GraphicsSettingsApplier`** — статические методы, меняющие графические настройки Unity.

Встроенные настройки:

| Имя в списке выбора | Тип | Что применяет |
|---|---|---|
| — | `ResolutionGameSetting` | разрешение, полноэкранный режим, частоту обновления |
| Graphics/Quality Level | `QualityLevelGameSettings` | уровень `QualitySettings` |
| Graphics/Shadows Game Settings | `ShadowsGameSettings` | тени вкл. / выкл. |
| Graphics/VSync | `VSyncGameSettings` | VSync вкл. / выкл. |
| Graphics/Frame Rate Cap | `FrameRateCapGameSettings` | `Application.targetFrameRate` |

## Использование

### Применение и изменение

```csharp
using RedCatEngine.GameSettings;
using RedCatEngine.GameSettings.TypeSettings.Graphics;

settingsService.LoadGameSetting();                                // по умолчанию, затем сохранённые изменения

settingsService.OverrideGameSetting(new VSyncGameSettings(true)); // применяется сразу
settingsService.OverrideGameSetting(new FrameRateCapGameSettings(60));
settingsService.SaveSettings();

settingsService.ResetSettings();                                  // сбросить изменения и применить настройки по умолчанию

settingsService.GameSettingChangeEvent += setting => { /* обновить интерфейс настроек */ };
```

### Собственная настройка

```csharp
using System;
using RedCatEngine.GameSettings.TypeSettings;
using SerializeReferenceEditor;
using UnityEngine;

[Serializable]
[SRName("Audio/Master Volume")]
public class MasterVolumeGameSetting : BaseGameSetting
{
	public float Volume = 1f;

	public override string SaveKey => nameof(MasterVolumeGameSetting);

	protected override void DoApply()
		=> AudioListener.volume = Volume;
}
```

Новый тип появляется в списке выбора у `GameVisualSettingsConfig.BaseSettings`, и его можно передать в `OverrideGameSetting`.

### Применение настройки напрямую

Настройке не нужен сервис, чтобы работать:

```csharp
new QualityLevelGameSettings(2).Apply();
GraphicsSettingsApplier.SetVSync(true);
```

## Известные ограничения

- `GameVisualSettingService` не инициализирует своё поле `OverrideSettingsData`, поэтому `LoadGameSetting`, `OverrideGameSetting`, `SaveSettings` и `ResetSettings` сейчас бросают `NullReferenceException`. Пока это не исправлено, применяйте настройки напрямую, как показано выше.
- `GameVisualSettingsConfig.Instance` опирается на `BaseSingleConfig`, который ищет в `Resources` ассет с буквальным именем `TConfig` — см. [Configs](../Configs/README.ru.md#известные-ограничения).
