# RandomEvents

A [MelonLoader](https://melonwiki.xyz/) mod for **Drova – Forsaken Kin** that adds level-scaled random encounters and regional ambushes throughout the game world.

Built on top of the [Drova Modding API](https://github.com/Drova-Modding/Drova-Modding-API).

> **Players:** see [docs/Encounters.md](docs/Encounters.md) for the full list of creatures, regions, level ranges and group sizes.
>
> **Modders / tinkerers:** every pool is a JSON file in `<game>/Mods/RandomEvents/` — see [Definitions/README.md](Definitions/README.md) and [Definitions/SPAWNABLES.md](Definitions/SPAWNABLES.md).

## What it does

While you play, the mod registers two kinds of events with the game's `WorldEventSystemManager`:

- **Global random events** – fired on the API's world-event timer regardless of where you are. Examples: *Wandering Beasts*, *Bandit Raid*, *Shadow Hunt*.
- **Regional ambushes** – fired when you enter a specific region. Each region has its own creature pool (Forest, Auwald, Leuchtwald, DeathMoor, RootenMoor, FriendlyMoor, River, Heide, Hain, Schlund, Cave, Mine, Ruins, Overworld, CityDungeon).

Every spawn is rolled fresh from a weighted, level-gated **encounter pool**, so:

- creatures only appear within their configured `minLevel`/`maxLevel` range
- the **number of distinct creature types** scales from `minTypesAtLow` (at lvl 1) up to `maxTypesAtHigh` (at lvl 40)
- the **count per creature** scales with `baseCount` + `countGrowth` as the player levels
- each regional pool has a `skipChance` so entering a region doesn't guarantee a fight

The intent is to keep the world feeling alive and dangerous from lvl 1 through the endgame without hand-placing every spawn.

## Tweaking encounters (no recompile)

All encounter content is data-driven JSON. After installing the mod, edit the files under:

```
<game>/Mods/RandomEvents/
  regions/<RegionName>.json    # triggered on region entry
  global/<AnyPoolName>.json    # rolled by the world-event timer
  encounter.schema.json        # validation schema
  SPAWNABLES.md                # full list of creatures / bandits / regions
  README.md                    # authoring guide
```

A pool file looks like this:

```jsonc
{
  "$schema": "../encounter.schema.json",
  "skipChance": 0.25,
  "minTypesAtLow": 1,
  "maxTypesAtHigh": 3,
  "entries": [
    { "creature": "Boar",                 "minLevel": 1,  "maxLevel": 20, "weight": 2.0, "baseCount": 1, "countGrowth": 2 },
    { "bandit":   "Human_Bandit_Mine_01", "minLevel": 5,  "maxLevel": 25, "weight": 1.5, "baseCount": 2, "countGrowth": 3 }
  ]
}
```

Key references:

| Need                                                   | File                                                                  |
| ------------------------------------------------------ | --------------------------------------------------------------------- |
| How the JSON works, file layout, schema fields         | [Definitions/README.md](Definitions/README.md)                        |
| Every valid `creature` / `bandit` / region name        | [Definitions/SPAWNABLES.md](Definitions/SPAWNABLES.md)                |
| JSON Schema (autocomplete + validation in your editor) | [Definitions/encounter.schema.json](Definitions/encounter.schema.json) |

The mod ships with the contents of [`Definitions/`](Definitions/README.md) as the source-of-truth content; on build, those files are copied to `<game>/Mods/RandomEvents/`. Reload the game after editing.

## Shipping your own pool pack (Nexus Mods)

You don't need to fork or recompile this mod to publish your own encounters. Author JSON pool files against [`encounter.schema.json`](Definitions/encounter.schema.json) and drop them next to the existing ones:

```
<game>/Mods/RandomEvents/
  regions/<RegionName>.json    # add or override a region pool
  global/<YourPoolName>.json   # add a new world-event pool
```

Package the `regions/` and/or `global/` files (and a short README) into a zip and upload it to [Nexus Mods](https://www.nexusmods.com/) — users just extract into their existing `Mods/RandomEvents/` folder.

Load-order notes:

- **Regional pools** are keyed by `Region` enum. If two files map to the same region (e.g. your `regions/Forest.json` and the bundled one), the **last file read wins** — your pack effectively *replaces* that region's pool.
- **Global pools** are additive. Every `global/*.json` file becomes its own pool that the world-event timer can roll, so your pack *adds* to what's already there.
- Unknown creature / bandit names log a warning in the MelonLoader log and are skipped — so a typo in your pack won't crash the game.

## Project layout

```
Core.cs                              MelonMod entry point — loads pools on scene load
Encounters/EncounterDefinitions.cs   In-memory pool store + Load() entry point
Encounters/EncounterLoader.cs        Reads regions/*.json + global/*.json next to the dll
Encounters/EncounterPool.cs          Weighted, level-gated pool builder
Encounters/CreatureEntry.cs          A single weighted entry (asset + level range + scaling)
Events/ScaledEncounterEvent.cs       Global event — re-rolls the spawn list each trigger
Events/ScaledRegionalEvent.cs        Regional event — re-rolls on region entry
Util/PlayerLevelHelper.cs            Reads current player level for scaling
Definitions/                         Source-of-truth JSON pools + schema + docs (deployed on build)
```

## Building

The csproj expects a `GamePath` MSBuild property pointing at your Drova install (set in `Directory.Build.props`), plus the `Drova_Modding_API.dll` in the game's `Mods/` folder. On build, the resulting `RandomEvents.dll` **and every file under [`Definitions/`](Definitions/README.md)** are copied into `$(GamePath)/Mods` automatically.

Requirements:

- .NET 6 SDK
- Drova – Forsaken Kin with MelonLoader installed
- [Drova Modding API](https://github.com/Drova-Modding/Drova-Modding-API) mod installed

## License

See [LICENSE](LICENSE).
