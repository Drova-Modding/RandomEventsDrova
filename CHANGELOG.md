# Changelog

## 1.4.1 - Spider Fix
- Glowing forest (Leuchtwald) events no longer spawn the `BigSpider_Leuchtwald_Spot_01` spider. That prefab can never die: it heals whenever its health reaches 0, so it always survived with one heart. The other Leuchtwald spider takes its spawn weight.

## 1.4.0 - Bandit Variety
- New bandit types for encounter files: `SwordBow`, `SpearBow`, `SwordNet`, `AxeNet`, `SpearNet`, `SwordKnives` and `SpearKnives` (net is the Fangnetz). `Random` picks them too. The Bandit Raid, Mine and Ruins events now spawn them from mid levels on.
- `Dagger` bandits fight with a sword: NPCs with a dagger could circle the player without ever striking.
- Requires Drova Modding API 0.9.0 or newer.

## 1.3.0 - Bandit Groups
- Bandits of one event fight as a group: once one of them fights, the others join in.
- Bandits notice the player from where they spawn and chase further, instead of walking to where the player stood when the event started.
- Requires Drova Modding API 0.8.0 or newer.

## 1.2.0 - Coop Update
- Added support for coop

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

