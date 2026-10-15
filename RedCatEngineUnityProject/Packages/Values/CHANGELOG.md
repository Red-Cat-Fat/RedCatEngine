# Changelog

All notable changes to Value package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.1] - 2026-10-14

### Fixed

- `CurveValue.MaxXValue` was marked `[SerializeReference]` and was not saved, so curves set up in the Inspector clamped X to `[Min, 0]` and *Curve/Repeat* could loop forever. It is now `[SerializeField]`; without a valid range the curve's first and last keys are used.
- *Curve/Repeat* wraps X with `Mathf.Repeat` over the range length, without loops.

### Changed

- *Comparisons/Equals* tolerance defaults to `Mathf.Epsilon` instead of `0`, which made a new value always `false`.

## [1.1.0] - 2026-10-12

### Added

- MultiConditionalFloatValue, Clamp operation.

### Changed

- MultiplierValueCalculationService, curve values, ConditionalFloatValue synchronized with the production code base.

## [1.0.1] - 2024-10-17

### Added

- Variable service.

### Changes

- ValueCalculationService.cs move to Service folder with namespace from **RedCatEngine.Values** to *
  *RedCatEngine.Values.Services**

## [1.0.0] - 2024-07-19

### Added

- Base services