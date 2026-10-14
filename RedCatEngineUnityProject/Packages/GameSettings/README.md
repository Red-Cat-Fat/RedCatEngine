**English** | [Русский](README.ru.md)

# Red Cat Engine: Game Settings

Graphics settings as data: default settings are listed in a config asset, player overrides are stored in a JSON file, and every setting knows how to apply itself.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.gamesettings` |
| Version | 2.0.0 ([changelog](CHANGELOG.md)) |
| Unity | 6000.0+ |
| Assembly | `GameSettings` |
| Namespace | `RedCatEngine.GameSettings` |

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/GameSettings
```

## Dependencies

| Package | Version | Used for |
|---|---|---|
| [Configs](../Configs/README.md) | 1.1.0 | `GameVisualSettingsConfig` |
| [Serialize Reference Editor](https://github.com/elmortem/serializereferenceeditor) | git | the `[SR]` type picker — add it to the project manually |

## Concepts

- **`BaseGameSetting`** — one setting: a `SaveKey` and `Apply()`. Override `DoApply()` and, if needed, `IsCanApply()`.
- **`GameVisualSettingsConfig`** — the asset with default settings (`BaseSettings`), created from **Configs/Common/Game Settings Config**.
- **`OverrideSettingsData`** — the player's overrides, saved to `<persistentDataPath>/<project>_overrideSettingsData.json`.
- **`GameVisualSettingService`** — applies defaults, then overrides; adds, saves and resets overrides; raises `GameSettingChangeEvent`.
- **`GraphicsSettingsApplier`** — static helpers that change Unity graphics settings.

Built-in settings:

| Picker name | Type | Applies |
|---|---|---|
| — | `ResolutionGameSetting` | resolution, fullscreen mode, refresh rate |
| Graphics/Quality Level | `QualityLevelGameSettings` | `QualitySettings` level |
| Graphics/Shadows Game Settings | `ShadowsGameSettings` | shadows on / off |
| Graphics/VSync | `VSyncGameSettings` | VSync on / off |
| Graphics/Frame Rate Cap | `FrameRateCapGameSettings` | `Application.targetFrameRate` |

## Usage

### Apply and override

```csharp
using RedCatEngine.GameSettings;
using RedCatEngine.GameSettings.TypeSettings.Graphics;

settingsService.LoadGameSetting();                                // defaults, then saved overrides

settingsService.OverrideGameSetting(new VSyncGameSettings(true)); // applied immediately
settingsService.OverrideGameSetting(new FrameRateCapGameSettings(60));
settingsService.SaveSettings();

settingsService.ResetSettings();                                  // drop overrides, reapply defaults

settingsService.GameSettingChangeEvent += setting => { /* refresh the settings UI */ };
```

### A custom setting

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

The new type appears in the picker of `GameVisualSettingsConfig.BaseSettings` and can be passed to `OverrideGameSetting`.

### Apply a setting directly

A setting does not need the service to work:

```csharp
new QualityLevelGameSettings(2).Apply();
GraphicsSettingsApplier.SetVSync(true);
```

## Known limitations

- `GameVisualSettingService` never initializes its `OverrideSettingsData` field, so `LoadGameSetting`, `OverrideGameSetting`, `SaveSettings` and `ResetSettings` currently throw `NullReferenceException`. Until this is fixed, apply settings directly as shown above.
- `GameVisualSettingsConfig.Instance` relies on `BaseSingleConfig`, which looks for a `Resources` asset literally named `TConfig` — see [Configs](../Configs/README.md#known-limitations).
