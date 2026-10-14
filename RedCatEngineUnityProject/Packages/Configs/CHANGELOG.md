# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.1] - 2026-10-14

### Fixed

- `BaseSingleConfig<TConfig>.Instance` looked for a `Resources` asset literally named `TConfig`. It now loads `Resources/Configs/<TypeName>` (the path it creates in the Editor), falling back to `Resources/<TypeName>`, and creates the missing folders before creating the asset.

## [1.1.0] - 2026-10-12

### Added

- Smoke tests.

### Changed

- BaseConfig, BaseConfigExtensions, ConfigID, config containers synchronized with the production code base.
