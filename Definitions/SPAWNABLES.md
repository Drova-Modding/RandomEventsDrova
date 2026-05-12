# Spawnable Reference

Every value you can put into an encounter pool entry. Names are **case-sensitive**
and come straight from `Drova_Modding_API.Access.AddressableAccess` and
`Drova_Modding_API.Systems.Region`.

If a name here is missing, it means the modding API does not expose it (yet) — the
mod can only spawn things that have an `AssetReferenceGameObject` field on
`Creatures` or `Bandits`.

---

## Regions

Used as the **filename** (without `.json`) of files in `regions/`.

| Name                | Notes                                                |
|---------------------|------------------------------------------------------|
| `RedTower`          | Endgame                                              |
| `Mine`              | cave region                                          |
| `Cave`              | cave region                                          |
| `City`              | probably a left over over city dungeon               |
| `SpiderDungeon`     | cave region                                          |
| `Auwald`            | floodplain forest                                    |
| `Nemeton`           | religious strict guys                                |
| `EntryNemeton`      | religious strict guys entry                          |
| `Intro`             | starting area                                        |
| `Ruins`             | cave region                                          |
| `Tavern`            | tavern                                               |
| `CityDungeon`       | cave region                                          |
| `DeathMoor`         | Deadly moor                                          |
| `Academy`           | cave region                                          |
| `Forest`            | The forest region over the tavern                    |
| `Library`           | cave region                                          |
| `FriendlyMoor`      | Most of the moor around Nemeton                      |
| `Mutter`            | "Mother" where the godess is from the glowing forest |
| `Leuchtwald`        | glowing forest                                       |
| `River`             | River region where you going hunt                    |
| `RootenMoor`        | rotten moor, flesh region                            |
| `WoodCamp`          | Cengiz and so on                                     |
| `RuinsCamp`         | The bad boys                                         |
| `RuinUnder`         | cave region                                          |
| `Magecamp`          | village in the glowing forest                        |
| `Ruinexplorer`      | cave region                                          |
| `RuinSchmuggler`    | cave region                                          |
| `Hain`              | grove                                                |
| `Heide`             | heath                                                |
| `Schlund`           | the Abyss                                            |
| `Overworld_Or_Cave` | fallback / default                                   |

---

## Creatures

Use as `"creature": "<name>"` in an entry.

### Beasts & fauna
`BabyRedFox`, `BabyRedFox_Outside`, `BabySnapper`, `Bear`, `Boar`, `Boar_Intro`,
`Boar_Luminous`, `Boar_Luminous_ExpeditionML`, `Boar_Luminous_ExpeditionRL`,
`DiggerPig`, `RedFox`, `Snapper`, `Snapper_ExtraWeak`, `Snapper_Weak`,
`WaterRat`, `WaterRat_Bone`, `WaterRat_Young`

### Rippers
`Ripper`, `Ripper_InstantAggro`, `HungryRipper`, `HungryRipper_InstantAggro`,
`EliteRipper`, `EliteRipper_HuntTheRipper`,
`ShadowRipper`, `ShadowRipper_Ambush`, `ShadowRipper_Ambush_LittleAggroRange`,
`ShadowRipper_Ambush_LittleAggroRange_NoReactionGrp`,
`ShadowRipper_ExpeditionRunFollower_02`, `ShadowRipper_ExpeditionRunLeader`

### Spiders
`BigSpider`, `BigSpider_AuwaldDungeon`, `BigSpider_Leuchtwald_Spot_01`,
`BigSpider_Leuchtwald_Spot_02`

### Spitters / Splitters
`Spitter`, `Spitter_Weak`, `Spitter_ExtraWeak`, `Spitter_ForestDungeonBoss`,
`Splitter_SpiderBossForest_Actual`, `Snapper_ForestDungeonBoss`,
`Snapper_SpiderBossForest_Actual`

### Frogs / bloodquito / worms
`Bloodquito_Auwald`, `Bloodquito_Kamikaze`, `Bloodquito_Rotmoor`,
`Bloodquito_RotmoorEnemiesWithFrogs`,
`Frog_Friendly_Hansi`, `Frog_Friendly_Player`, `Frog_Melee`, `Frog_Ranged`,
`Worm`

### Sprigga / plants
`Sprigga`, `Sprigga_Yellow`, `Sprigga_Young_Hidden`, `Sprigga_Young_Normal`,
`Sprigga_Young_Yellow_Hidden`, `Sprigga_Young_Yellow_Normal`, `MortarShroom`

### Water creatures
`WaterAmbusher`, `WaterDevil`, `WaterDevil_Red`

### Foglet / Stalker / Harpy / misc
`Foglet`, `Foglet_Weak`, `Foglet_WithoutFog`,
`Stalker_Blue`, `Stalker_Green`, `Stalker_Red`,
`Harpy`, `Banshee`, `Menshee`, `Leshen`, `ChaosSpirit`

### Golem / StoneFist
`Golem`, `Golem_ChallengeCave`, `Golem_Library`,
`StoneFist`, `StoneFist_BigGroupAggro`, `StoneFist_DeepMine`

### LightBear
`LightBear`, `LightBear_ExpeditionML`

### GreatTusk + minions
`GreatTusk`,
`Boar_Luminous_GreatTuskMinion`, `Boar_Luminous_GreatTuskMinion_EndPhase`,
`Boar_Luminous_GreatTuskMinion_Intimidate`,
`LightBear_GreatTuskMinion`, `ShadowRipper_GreatTuskMinion`

### EnemyFlesh
`EnemyFlesh_Baneling`, `EnemyFlesh_Melee`, `EnemyFlesh_Ranged`,
`EnemyFlesh_Plant`, `EnemyFlesh_Tank`

### Draugr
`Draugr_Axe`, `Draugr_Mage`, `Draugr_Mage_BrutusCave`,
`Draugr_Mage_RedTower_Stage_03_Letter`,
`Draugr_Spear`, `Draugr_Spear_BrutusCave`, `Draugr_Spear_BrutusCave_StartAlive`,
`Draugr_Spear_RedTower_Letter`, `Draugr_Spear_RedTower_Stage_03`,
`Draugr_Sword_Library_Letter`, `Draugr_Sword_Shield`,
`Draugr_Sword_Shield_BrutusCave`, `Draugr_Sword_Shield_BrutusCave_StartAlive`,
`Draugr_Sword_Shield_DraugrDungeonIntro`, `Draugr_Sword_Shield_RedTower_Stage_03`

### Bosses / story creatures
`Brutus`, `BrutusEnd_BigSpider`, `BrutusEnd_Outro_Creatures`

### Critters (mostly cosmetic — passive or run-away AI)
`Critter_Cat_Aggressive_01`, `Critter_Cat_Aggressive_02`,
`Critter_Cat_Lovely_01`, `Critter_Cat_Lovely_02`,
`Critter_Cat_RunAway_01`, `Critter_Cat_RunAway_02`,
`Critter_Mikesch`,
`Critter_Dog_Type1_01`, `Critter_Dog_Type1_02`,
`Critter_Dog_Type2_01`, `Critter_Dog_Type2_02`,
`Critter_Dog_Type3_01`, `Critter_Dog_Type3_02`,
`Critter_BlackGrouse_Female`, `Critter_BlackGrouse_Male`,
`Critter_NurturingSprite`,
`Critter_Sheep_NonDialog`, `Critter_Sheep_01`, `Critter_Sheep_02`,
`Critter_Sheep_03`, `Critter_Sheep_04`, `Critter_Sheep_Generic_01`,
`Critter_Rat`, `Critter_Snake_Adder`

---

## Bandits

Use as `"bandit": "<name>"` in an entry.

| Name                         | Theme                  |
|------------------------------|------------------------|
| `Human_Bandit_Expedition_01` | expedition / overworld |
| `Human_Bandit_Expedition_02` | expedition / overworld |
| `Human_Bandit_Expedition_03` | expedition / overworld |
| `Human_Bandit_Expedition_04` | expedition / overworld |
| `Human_Bandit_Mine_01`       | mine                   |
| `Human_Bandit_Mine_02`       | mine                   |
| `Human_Bandit_Mine_03`       | mine                   |
| `Human_Bandit_RuinKasern_01` | barracks ruin          |
| `Human_Bandit_RuinKasern_02` | barracks ruin          |
| `Human_Bandit_RuinKasern_03` | barracks ruin          |
| `Human_Bandit_Ruins_01`      | ruins                  |
| `Human_Bandit_Ruins_02`      | ruins                  |
| `Human_Bandit_Ruins_03`      | ruins                  |
| `Human_Bandit_Ruins_04`      | ruins                  |
| `Human_Bandit_Ruins_05`      | ruins                  |
| `Human_Bandit_Snaga`         | Snaga variant          |

---

Source of truth: `Drova_Modding_API.Access.AddressableAccess.Creatures`,
`.Bandits`, and `Drova_Modding_API.Systems.Region` in the Drova Modding API.
