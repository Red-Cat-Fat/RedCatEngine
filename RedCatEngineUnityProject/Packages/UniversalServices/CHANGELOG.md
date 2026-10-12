# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-10-12

### Added

- New log system: LogHub, LogRouter, sinks, filtering, tag catching, log filter and viewer editor windows.
- Save, Tags, MouseLockerServices, UpdatableService services and Utils.
- DoTweenExtensions (compiled only when DOTWEEN is defined).

### Changed

- BREAKING: ILogService and log implementations reworked.
- Log filter storage keys renamed to RedCatEngine.Log.*.

### Removed

- UnityEditorLogService and related editor window (replaced by the new log system).
- GameDayTimeService, IGameDayTimeService, SunRotator.
