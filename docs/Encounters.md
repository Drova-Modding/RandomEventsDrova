# Encounters Guide

A player-friendly overview of every encounter the **RandomEvents** mod can throw at you.

- **Level range** — the player levels at which a creature may appear. Outside this range it simply won't show up.
- **Weight** — the relative chance of being picked when the pool rolls (higher = more common).
- **Group size** — the number of creatures spawned, shown as *low level → high level*. Larger groups appear as you level up.

Per-region pools also have a **Skip Chance**: the probability that entering the region produces **no** encounter at all. This keeps regional ambushes from feeling spammy.

---

## Global random events

These can fire anywhere in the world. The world-event timer picks one of these pools at random.

### Wandering Beasts

| Creature | Level range | Weight | Group size |
|----------|-------------|--------|------------|
| Boar     | 8 – 20      | 2.0    | 1 → 5      |
| Ripper   | 5 – 30      | 1.5    | 1 → 9      |
| Snapper  | 1 – 30      | 1.0    | 1 → 6      |
| Bear     | 18 – 40     | 0.5    | 1 → 4      |

### Bandit Raid

| Creature             | Level range | Weight | Group size |
|----------------------|-------------|--------|------------|
| Bandit Expedition 01 | 3 – 20      | 1.5    | 2 → 6      |
| Bandit Expedition 02 | 6 – 25      | 1.5    | 1 → 5      |
| Bandit Expedition 03 | 12 – 32     | 1.0    | 1 → 5      |
| Bandit Expedition 04 | 18 – 40     | 0.8    | 1 → 5      |

### Shadow Hunt

| Creature               | Level range | Weight | Group size |
|------------------------|-------------|--------|------------|
| Shadow Ripper          | 20 – 40     | 1.5    | 1 → 6      |
| Shadow Ripper (Ambush) | 22 – 40     | 1.0    | 1 → 6      |
| Elite Ripper           | 30 – 40     | 0.5    | 1 → 11     |

---

## Regional ambushes

Triggered when the player enters the region. Each pool has a **Skip Chance** so not every entry leads to a fight.

### Forest *(Skip 25%)*

| Creature      | Level range | Weight | Group size |
|---------------|-------------|--------|------------|
| Boar          | 1 – 20      | 2.0    | 1 → 3      |
| Red Fox       | 1 – 15      | 1.0    | 1 → 2      |
| Ripper        | 5 – 30      | 1.5    | 1 → 3      |
| Hungry Ripper | 12 – 40     | 1.0    | 1 → 3      |
| Bear          | 18 – 40     | 0.6    | 1 → 2      |

### Auwald *(Skip 25%)*

| Creature            | Level range | Weight | Group size |
|---------------------|-------------|--------|------------|
| Bloodquito (Auwald) | 6 – 30      | 2.0    | 2 → 6      |
| Big Spider          | 10 – 35     | 1.5    | 1 → 3      |
| Sprigga (Young)     | 8 – 25      | 1.0    | 2 → 4      |
| Sprigga             | 20 – 40     | 0.5    | 1 → 2      |

### Leuchtwald *(Skip 25%)*

| Creature             | Level range | Weight | Group size |
|----------------------|-------------|--------|------------|
| Big Spider (Spot 01) | 12 – 35     | 1.0    | 1 → 3      |
| Big Spider (Spot 02) | 12 – 35     | 1.0    | 1 → 3      |
| Luminous Boar        | 15 – 40     | 1.5    | 1 → 3      |
| Light Bear           | 22 – 40     | 0.7    | 1 → 2      |
| Snapper              | 1 – 30      | 1.0    | 1 → 3      |
| Mortar Shroom        | 1 – 30      | 1.0    | 1 → 3      |

### Deathmoor *(Skip 50%)*

| Creature          | Level range | Weight | Group size |
|-------------------|-------------|--------|------------|
| Foglet            | 8 – 30      | 2.0    | 2 → 6      |
| Foglet (Weak)     | 1 – 15      | 1.5    | 2 → 5      |
| Banshee           | 18 – 40     | 0.8    | 1 → 2      |
| Menshee           | 22 – 40     | 0.5    | 1 → 2      |
| Water Ambusher    | 22 – 40     | 0.5    | 1 → 2      |
| Water Devil       | 20 – 35     | 1.0    | 1 → 2      |
| Water Devil (Red) | 20 – 35     | 0.5    | 1 → 2      |

### Rootenmoor *(Skip 50%)*

| Creature       | Level range | Weight | Group size |
|----------------|-------------|--------|------------|
| Flesh Baneling | 20 – 30     | 2.0    | 2 → 6      |
| Flesh Melee    | 20 – 30     | 1.5    | 2 → 5      |
| Flesh Plant    | 20 – 30     | 1.5    | 1 → 3      |
| Flesh Ranged   | 20 – 30     | 1.0    | 1 → 2      |
| Flesh Tank     | 25 – 30     | 1.0    | 1 → 2      |

### Friendly Moor *(Skip 75%)*

| Creature             | Level range | Weight | Group size |
|----------------------|-------------|--------|------------|
| Frog (Melee)         | 1 – 18      | 1.0    | 1 → 3      |
| Frog (Ranged)        | 1 – 15      | 1.0    | 2 → 4      |
| Bloodquito (Rotmoor) | 1 – 15      | 1.0    | 2 → 4      |
| Harpy                | 25 – 40     | 1.0    | 1 → 3      |
| Foglet (Weak)        | 5 – 40      | 1.0    | 2 → 6      |
| Foglet               | 20 – 40     | 1.0    | 2 → 6      |

### River *(Skip 25%)*

| Creature          | Level range | Weight | Group size |
|-------------------|-------------|--------|------------|
| Water Ambusher    | 25 – 30     | 1.5    | 1 → 3      |
| Water Devil       | 20 – 35     | 1.0    | 1 → 2      |
| Water Devil (Red) | 25 – 40     | 0.5    | 1 → 2      |
| Water Rat         | 1 – 12      | 1.5    | 2 → 4      |
| Frog (Melee)      | 10 – 20     | 1.5    | 2 → 4      |
| Frog (Ranged)     | 10 – 20     | 1.5    | 2 → 4      |

### Heide *(Skip 25%)*

| Creature      | Level range | Weight | Group size |
|---------------|-------------|--------|------------|
| Boar          | 5 – 18      | 1.5    | 1 → 3      |
| Great Tusk    | 28 – 40     | 0.3    | 1 → 3      |
| Shadow Ripper | 28 – 40     | 0.3    | 1 → 4      |
| Bear          | 10 – 40     | 0.3    | 1 → 4      |

### Hain *(Skip 25%)*

| Creature      | Level range | Weight | Group size |
|---------------|-------------|--------|------------|
| Stalker (Red) | 18 – 40     | 1.0    | 1 → 3      |
| Hungry Ripper | 8 – 30      | 1.0    | 1 → 3      |

### Schlund *(Skip 25%)*

| Creature               | Level range | Weight | Group size |
|------------------------|-------------|--------|------------|
| Shadow Ripper          | 20 – 40     | 1.5    | 1 → 3      |
| Shadow Ripper (Ambush) | 25 – 40     | 1.0    | 1 → 3      |
| Stalker (Red)          | 25 – 40     | 1.0    | 1 → 2      |
| Elite Ripper           | 30 – 40     | 0.5    | 1 → 2      |
| Water Rat (Young)      | 1 – 10      | 0.5    | 1 → 2      |
| Bloodquito (Auwald)    | 1 – 10      | 0.5    | 1 → 2      |
| Water Rat              | 1 – 10      | 0.5    | 1 → 2      |

### Cave *(Skip 25%)*

| Creature   | Level range | Weight | Group size |
|------------|-------------|--------|------------|
| Big Spider | 8 – 40      | 1.5    | 1 → 6      |
| Worm       | 15 – 40     | 1.0    | 1 → 5      |
| Stone Fist | 8 – 40      | 0.8    | 1 → 4      |

### Mine *(Skip 25%)*

| Creature               | Level range | Weight | Group size |
|------------------------|-------------|--------|------------|
| Stone Fist             | 12 – 35     | 1.0    | 1 → 3      |
| Stone Fist (Deep Mine) | 22 – 40     | 0.7    | 1 → 2      |
| Mine Bandit 01         | 5 – 25      | 1.5    | 2 → 5      |
| Mine Bandit 02         | 8 – 28      | 1.2    | 1 → 3      |
| Mine Bandit 03         | 14 – 35     | 1.0    | 1 → 3      |

### Ruins *(Skip 25%)*

| Creature              | Level range | Weight | Group size |
|-----------------------|-------------|--------|------------|
| Ruin Bandit Kasern 01 | 20 – 40     | 2.0    | 2 → 5      |
| Ruin Bandit Kasern 02 | 20 – 40     | 1.5    | 2 → 5      |
| Ruin Bandit Kasern 03 | 20 – 40     | 1.0    | 1 → 3      |

### Overworld *(Skip 25%)*

Fallback pool for any region without its own list.

| Creature | Level range | Weight | Group size |
|----------|-------------|--------|------------|
| Boar     | 6 – 20      | 2.0    | 1 → 3      |
| Snapper  | 1 – 30      | 1.0    | 1 → 3      |
| Spitter  | 8 – 30      | 1.0    | 1 → 3      |

### City Dungeon *(Skip 25%)*

| Creature                | Level range | Weight | Group size |
|-------------------------|-------------|--------|------------|
| Draugr (Axe)            | 15 – 30     | 1.5    | 1 → 4      |
| Draugr (Mage)           | 15 – 35     | 1.0    | 1 → 4      |
| Draugr (Spear)          | 15 – 32     | 1.0    | 1 → 4      |
| Draugr (Sword & Shield) | 15 – 35     | 0.8    | 1 → 4      |

---

## How scaling works in plain terms

1. **Level filter** — only creatures whose level range covers your current level are considered.
2. **Type count** — the pool decides how many *different* creatures to spawn. At low level it picks just 1 type; at level 40 it can pick up to the pool's maximum (usually 3, sometimes more).
3. **Weighted pick** — each eligible creature is rolled by its weight. Higher weight = more common.
4. **Group size** — each picked creature spawns a group whose size grows linearly with your level, from the *low-level* count up to the *high-level* count shown in the tables above.

The result is that early game encounters are small and forgiving, and late-game encounters become large multi-type ambushes.
