# RandomEvents — Encounter Definitions

Each JSON file in this folder defines one **encounter pool**. The mod loads them
at startup from `<game>/Mods/RandomEvents/` (next to `RandomEvents.dll`).

## Layout

```
RandomEvents/
  encounter.schema.json    # JSON Schema — editors validate against this
  SPAWNABLES.md            # full list of creature/bandit/region names
  README.md
  regions/
    Forest.json            # filename MUST match a Drova Region enum value
    Auwald.json
    ...
  global/
    Wandering Beasts.json  # filename = pool name (free-form)
    Bandit Raid.json
    ...
```

- Files under `regions/` are triggered when the player enters the matching
  region. The filename (without `.json`) **must** be a valid
  `Drova_Modding_API.Systems.Region` value — see [SPAWNABLES.md](./SPAWNABLES.md).
- Files under `global/` are rolled by the world-event timer regardless of
  region. The filename becomes the pool's display name.

## Validation

Every pool file should start with:

```json
"$schema": "../encounter.schema.json"
```

VS Code, Rider, IntelliJ, and most JSON editors will then validate the file
against [`encounter.schema.json`](./encounter.schema.json), auto-complete
creature/bandit names from the enum, and flag typos in field names.

## Schema (summary)

```jsonc
{
  "$schema": "../encounter.schema.json",

  // Chance the pool is skipped entirely when triggered (0.0–1.0). Default 0.25.
  "skipChance": 0.25,

  // Distinct types picked per spawn at player level 1.
  "minTypesAtLow": 1,

  // Distinct types picked per spawn at player level 40.
  "maxTypesAtHigh": 3,

  "entries": [
    {
      // Exactly one of "creature" or "bandit" — case-sensitive field name on
      // AddressableAccess.Creatures or AddressableAccess.Bandits.
      "creature": "Boar",

      "minLevel":   1,    // earliest player level the entry can appear (1..40)
      "maxLevel":   20,   // latest player level the entry can appear  (1..40)
      "weight":     2.0,  // selection weight (higher = more likely)
      "baseCount":  1,    // group size at minLevel
      "countGrowth": 2    // extra added linearly up to maxLevel
    },
    { "bandit": "Human_Bandit_Mine_01", "minLevel": 5, "maxLevel": 25, "weight": 1.5, "baseCount": 2, "countGrowth": 3 }
  ]
}
```

JSON comments (`// ...`) and trailing commas are allowed in the loader. Some
editors complain about them under strict JSON — use a `.jsonc` view if so.

## What can I spawn? Which regions exist?

See [**SPAWNABLES.md**](./SPAWNABLES.md) — full list of every valid creature
name, bandit name, and region.

Unknown names log a warning in the MelonLoader log and the entry is skipped.
