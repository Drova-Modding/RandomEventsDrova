# RandomEvents

A [MelonLoader](https://melonwiki.xyz/) mod for **Drova – Forsaken Kin** that adds level-scaled random encounters and regional ambushes throughout the game world.

Built on top of the [Drova Modding API](https://github.com/Drova-Modding/Drova-Modding-API).

> **Players:** see [docs/Encounters.md](docs/Encounters.md) for the full list of creatures, regions, level ranges and group sizes.

## What it does

While you play, the mod registers two kinds of events with the game's `WorldEventSystemManager`:

- **Global random events** – fired on the API's world-event timer regardless of where you are. Examples: *Wandering Beasts*, *Bandit Raid*, *Shadow Hunt*.
- **Regional ambushes** – fired when you enter a specific region. Each region has its own creature pool (Forest, Auwald, Leuchtwald, DeathMoor, RootenMoor, FriendlyMoor, River, Heide, Hain, Schlund, Cave, Mine, Ruins, Overworld, CityDungeon).

Every spawn is rolled fresh from a weighted, level-gated **encounter pool**, so:

- creatures only appear within their configured `minLevel`/`maxLevel` range
- the **number of distinct creature types** scales from `MinTypesAtLow` (at lvl 1) up to `MaxTypesAtHigh` (at lvl 40)
- the **count per creature** scales with `baseCount` + `countGrowth` as the player levels
- each regional pool has a `SkipChance` so entering a region doesn't guarantee a fight

The intent is to keep the world feeling alive and dangerous from lvl 1 through the endgame without hand-placing every spawn.

## Project layout

```
Core.cs                              MelonMod entry point — registers pools on scene load
Encounters/EncounterDefinitions.cs   Static config: which creatures spawn where, weights, level ranges
Encounters/EncounterPool.cs          Weighted, level-gated pool builder
Encounters/CreatureEntry.cs          A single weighted entry (asset + level range + scaling)
Events/ScaledEncounterEvent.cs       Global event — re-rolls the spawn list each trigger
Events/ScaledRegionalEvent.cs        Regional event — re-rolls on region entry
Util/PlayerLevelHelper.cs            Reads current player level for scaling
```

## Building

The csproj expects a `GamePath` MSBuild property pointing at your Drova install (set in `Directory.Build.props`), plus the `Drova_Modding_API.dll` in the game's `Mods/` folder. On build, the resulting `RandomEvents.dll` is copied into `$(GamePath)/Mods` automatically.

Requirements:

- .NET 6 SDK
- Drova – Forsaken Kin with MelonLoader installed
- [Drova Modding API](https://github.com/Drova-Modding/Drova-Modding-API) mod installed

## Tweaking encounters

All encounter content lives in `Encounters/EncounterDefinitions.cs`. Each entry is one fluent line:

```csharp
RegionalPools[Region.Forest] = new EncounterPool("Forest")
    .Add(AddressableAccess.Creatures.Boar,   minLevel: 1, maxLevel: 20, weight: 2f, baseCount: 1, countGrowth: 2)
    .Add(AddressableAccess.Creatures.Bear,   minLevel: 18, maxLevel: 40, weight: 0.6f, baseCount: 1, countGrowth: 1);
```

Optional per-pool knobs: `SkipChance`, `MinTypesAtLow`, `MaxTypesAtHigh`.

Available prefab references live on `AddressableAccess.Creatures` and `AddressableAccess.Bandits` (provided by the Drova Modding API).

## License

See [LICENSE](LICENSE).
