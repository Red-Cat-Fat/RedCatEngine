# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-10-12

### Changed

- BREAKING: synchronized with the production code base. WindowService, LayerContainer, WindowCreatorFactory, WindowData, IWindowService, IWindowData reworked.

### Removed

- IWindowContainer, WindowContainer and the parent-window cycle detection tests.
