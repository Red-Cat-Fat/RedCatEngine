# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.2] - 2026-10-14

### Fixed

- Cyclic parent windows (`A` → `B` → `A`) no longer recurse until a stack overflow: opening stops at the window that is already being opened and logs an error.

## [2.0.1] - 2026-10-12

### Fixed

- Assembly renamed `Windows` -> `MvpWindows` (asmdef GUID unchanged). Unity's `BuildPlayerDataGenerator`
  (`UnitySerializationLogic.ShouldNotTryToResolve`) hard-codes the assembly name `Windows` as the WinRT metadata
  assembly and refuses to resolve any type whose scope is named `Windows`. As a result every serialized field typed
  `WindowConfig` / `WindowConfig[]` declared in another assembly was dropped from the player TypeDB, the player read
  the objects with the real class layout and crashed on startup with
  `The file 'sharedassets0.assets' is corrupted! [Position out of bounds!]`.
  Consumers must update their asmdef references from `Windows` to `MvpWindows` (or reference by GUID).

## [2.0.0] - 2026-10-12

### Changed

- BREAKING: synchronized with the production code base. WindowService, LayerContainer, WindowCreatorFactory, WindowData, IWindowService, IWindowData reworked.

### Removed

- IWindowContainer, WindowContainer and the parent-window cycle detection tests.
