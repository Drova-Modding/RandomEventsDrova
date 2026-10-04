# Changelog

## 1.1.4 – Regional spawn fixes

- Fixed the regional re-entry cooldown being pushed out by every blocked attempt, which could keep a region from ever firing while the player crossed its border repeatedly.
- Encounter definitions are re-read on every gameplay load, so edited JSON files no longer need a full game restart.

## 1.1.3 – Cave & boss area toggles

- Events no longer fire in caves and dungeons, or in the regions that stage a boss fight.
- Two switches in the options menu (Modding tab) turn each block off again. Both defaults are true.

## 1.1.2 – Bugfix release
- Fixed Intro Region could spawn events.

## 1.1.1 – Modding API Compatibility

- Updated to be compatible with the latest Drova Modding API (v0.5.0).

## 1.1.0 – Bandit Routines Update

- Bandits now have by default a routine to rotate between spawn point and current player position
- Switched `BanditEntry` to spawn `LazyActor` bandits directly (no `GameObject` return).
- Added `SpawnTracker` utility to manage mixed spawned entities (`GameObject` + `LazyActor`) in one place.

