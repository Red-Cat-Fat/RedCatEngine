# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-10-12

### Added

- ITypedQueueStateMachine.ClearQueue.

### Changed

- BREAKING: QueueStateIsEmptyException removed, entering the next state from an empty queue is no longer an error.
- State names in logs are shortened.
