# Changelog

## 1.1.2 – Bugfix release
- Fixed Intro Region could spawn events.

## 1.1.1 – Modding API Compatibility

- Updated to be compatible with the latest Drova Modding API (v0.5.0).

## 1.1.0 – Bandit Routines Update

- Bandits now have by default a routine to rotate between spawn point and current player position
- Switched `BanditEntry` to spawn `LazyActor` bandits directly (no `GameObject` return).
- Added `SpawnTracker` utility to manage mixed spawned entities (`GameObject` + `LazyActor`) in one place.

