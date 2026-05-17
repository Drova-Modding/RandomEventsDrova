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

  // Chance the pool is skipped entirely when triggered (0.0–1.0). Default 0.25. Only has effect on region pools.
  "skipChance": 0.25,

  // Distinct types picked per spawn at player level 1.
  "minTypesAtLow": 1,

  // Distinct types picked per spawn at player level 40.
  "maxTypesAtHigh": 3,

  "entries": [
    {
      // Option A — asset-backed creature (AddressableAccess.Creatures field name).
      "creature": "Boar",

      "minLevel":    1,    // earliest player level the entry can appear (1..40)
      "maxLevel":    20,   // latest player level the entry can appear  (1..40)
      "weight":      2.0,  // selection weight (higher = more likely)
      "baseCount":   1,    // group size at minLevel
      "countGrowth": 2     // extra members added linearly up to maxLevel
    },
    {
      // Option B — BanditCreator bandit (recommended for all new bandit pools).
      // The NPC is built at runtime with role-appropriate gear and randomised cosmetics.
      "banditType":       "SwordShield", // see bandit-type table below
      "banditDifficulty": "Normal",      // Easy | Normal | Hard  (default: Normal)

      "minLevel":    8,
      "maxLevel":    25,
      "weight":      1.5,
      "baseCount":   1,
      "countGrowth": 3
    }
  ]
}
```

JSON comments (`// ...`) and trailing commas are allowed in the loader. Some
editors complain about them under strict JSON — use a `.jsonc` view if so.

### Entry types — quick reference

Each entry must use **exactly one** of the three spawn strategies:

| Field          | Strategy                     | Source                              |
|----------------|------------------------------|-------------------------------------|
| `creature`     | Asset-backed creature        | `AddressableAccess.Creatures`       |
| `bandit`       | Asset-backed bandit (bugged) | `AddressableAccess.Bandits`         |
| `banditType`   | BanditCreator bandit         | `BanditCreator` factory methods     |

### BanditCreator — `banditType` values

| `banditType`       | Weapon loadout                           |
|--------------------|------------------------------------------|
| `Random`           | Random choice from all archetypes        |
| `Dagger`           | Dagger                                   |
| `Sword`            | Sword                                    |
| `Axe`              | Axe                                      |
| `SwordShield`      | Sword + Shield                           |
| `Spear`            | Spear                                    |
| `SpearShield`      | Spear + Shield                           |
| `Bow`              | Bow + Quiver                             |
| `SpearSlingshot`   | Spear + Slingshot (mixed melee/ranged)   |
| `SwordSlingshot`   | Sword + Slingshot (mixed melee/ranged)   |

### BanditCreator — `banditDifficulty` values

| `banditDifficulty` | Effect                                                      |
|--------------------|-------------------------------------------------------------|
| `Easy`             | Coarse / improvised weapons, ragged armour                  |
| `Normal` (default) | Standard bandit weapons and T1/T2 armour                    |
| `Hard`             | Quality weapons, T4 armour, heavier shields + bonus talents |

Bandits spawn with randomized hair, beard, scars, and dirt layers regardless of difficulty.
Difficulty also scales which talents the bandit has unlocked.

## What can I spawn? Which regions exist?

See [**SPAWNABLES.md**](./SPAWNABLES.md) — full list of every valid creature
name, legacy bandit name, and region.

Unknown names and unrecognised `banditType`/`banditDifficulty` values log a
warning in the MelonLoader log and the entry is skipped.
