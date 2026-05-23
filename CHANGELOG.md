# Changelog

## 1.1.0 – Bandit Routines Update

- Bandits now have by default a routine to rotate between spawn point and current player position
- Switched `BanditEntry` to spawn `LazyActor` bandits directly (no `GameObject` return).
- Added `SpawnTracker` utility to manage mixed spawned entities (`GameObject` + `LazyActor`) in one place.

