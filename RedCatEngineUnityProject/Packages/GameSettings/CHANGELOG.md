# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.1] - 2026-10-14

### Fixed

- `GameVisualSettingService` never created its `OverrideSettingsData`, so every method threw `NullReferenceException`.
- Player overrides are serialized with `[SerializeReference]`; the abstract `BaseGameSetting` list was not saved before.
- Overriding a setting replaces the previous override with the same `SaveKey` instead of appending a duplicate.

## [2.0.0] - 2026-10-12

### Added

- Smoke tests.

### Changed

- BREAKING: GameSettingService renamed to GameVisualSettingService, GameSettingsConfig renamed to GameVisualSettingsConfig (script GUIDs are preserved).
- Minimal Unity version raised to 6000.0.
