using Drova_Modding_API.Access;
using Drova_Modding_API.Systems;

namespace RandomEvents.Encounters
{
    /// <summary>
    /// Placeholder encounter pools per region and a global "wandering" pool.
    /// Fill these out — each line is just a starting suggestion. Adjust min/max levels, weights and counts to taste.
    /// AddressableAccess.Creatures and AddressableAccess.Bandits hold all known prefab references.
    /// </summary>
    public static class EncounterDefinitions
    {
        /// <summary>
        /// Region-specific encounter pools. The WorldEventSystemManager triggers these when the player enters the region.
        /// </summary>
        public static readonly Dictionary<Region, EncounterPool> RegionalPools = new();

        /// <summary>
        /// Global pools rolled by the random world-event timer regardless of region.
        /// Add multiple pools so the timer can pick variety (e.g. "wolves", "spiders", "bandits").
        /// </summary>
        public static readonly List<EncounterPool> GlobalPools = new();

        public static void BuildPlaceholders()
        {
            BuildRegionPlaceholders();
            BuildGlobalPlaceholders();
        }

        private static void BuildRegionPlaceholders()
        {
            // -----------------------------------------------------------------
            // FOREST — early-mid game woodland creatures
            // -----------------------------------------------------------------
            RegionalPools[Region.Forest] = new EncounterPool("Forest")
                .Add(AddressableAccess.Creatures.Boar,        minLevel: 1,  maxLevel: 20, weight: 2f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.RedFox,      minLevel: 1,  maxLevel: 15, weight: 1f, baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.Ripper,      minLevel: 5,  maxLevel: 30, weight: 1.5f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.HungryRipper, minLevel: 12, maxLevel: 40, weight: 1f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Bear,        minLevel: 18, maxLevel: 40, weight: 0.6f, baseCount: 1, countGrowth: 1)
                ;

            // -----------------------------------------------------------------
            // AUWALD — mystical glow-forest, harder fauna
            // -----------------------------------------------------------------
            RegionalPools[Region.Auwald] = new EncounterPool("Auwald")
                .Add(AddressableAccess.Creatures.Bloodquito_Auwald, minLevel: 6,  maxLevel: 30, weight: 2f, baseCount: 2, countGrowth: 4)
                .Add(AddressableAccess.Creatures.BigSpider,         minLevel: 10, maxLevel: 35, weight: 1.5f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Sprigga_Young_Normal, minLevel: 8, maxLevel: 25, weight: 1f, baseCount: 2, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Sprigga,           minLevel: 20, maxLevel: 40, weight: 0.5f, baseCount: 1, countGrowth: 1)
                ;

            // -----------------------------------------------------------------
            // LEUCHTWALD — deeper glow forest
            // -----------------------------------------------------------------
            RegionalPools[Region.Leuchtwald] = new EncounterPool("Leuchtwald")
                .Add(AddressableAccess.Creatures.BigSpider_Leuchtwald_Spot_01, minLevel: 12, maxLevel: 35, weight: 1f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.BigSpider_Leuchtwald_Spot_02, minLevel: 12, maxLevel: 35, weight: 1f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Boar_Luminous,                minLevel: 15, maxLevel: 40, weight: 1.5f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.LightBear,                    minLevel: 22, maxLevel: 40, weight: 0.7f, baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.Snapper, minLevel: 1,  maxLevel: 30, weight: 1f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.MortarShroom, minLevel: 1,  maxLevel: 30, weight: 1f, baseCount: 1, countGrowth: 2)

                ;

            // -----------------------------------------------------------------
            // DEATHMOOR / ROOTENMOOR — undead-ish swamp
            // -----------------------------------------------------------------
            RegionalPools[Region.DeathMoor] = new EncounterPool("DeathMoor")
                .Add(AddressableAccess.Creatures.Foglet,                minLevel: 8,  maxLevel: 30, weight: 2f, baseCount: 2, countGrowth: 4)
                .Add(AddressableAccess.Creatures.Foglet_Weak,           minLevel: 1,  maxLevel: 15, weight: 1.5f, baseCount: 2, countGrowth: 3)
                .Add(AddressableAccess.Creatures.Banshee,               minLevel: 18, maxLevel: 40, weight: 0.8f, baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.Menshee,               minLevel: 22, maxLevel: 40, weight: 0.5f, baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.WaterAmbusher,               minLevel: 22, maxLevel: 40, weight: 0.5f, baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.WaterDevil,    minLevel: 20, maxLevel: 35, weight: 1f,   baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.WaterDevil_Red,    minLevel: 20, maxLevel: 35, weight: 0.5f, baseCount: 1, countGrowth: 1)
                ;
            RegionalPools[Region.DeathMoor].SkipChance = 0.5f;

            RegionalPools[Region.RootenMoor] = new EncounterPool("RootenMoor")
                .Add(AddressableAccess.Creatures.EnemyFlesh_Baneling,                  minLevel: 20,  maxLevel: 30, weight: 2f, baseCount: 2, countGrowth: 4)
                .Add(AddressableAccess.Creatures.EnemyFlesh_Melee,  minLevel: 20,  maxLevel: 30, weight: 1.5f, baseCount: 2, countGrowth: 3)
                .Add(AddressableAccess.Creatures.EnemyFlesh_Plant,                          minLevel: 20,  maxLevel: 30, weight: 1.5f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.EnemyFlesh_Ranged,                         minLevel: 20,  maxLevel: 30, weight: 1f,   baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.EnemyFlesh_Tank,                         minLevel: 25,  maxLevel: 30, weight: 1f,   baseCount: 1, countGrowth: 1)
                ;
            RegionalPools[Region.RootenMoor].SkipChance = 0.5f;


            // -----------------------------------------------------------------
            // FRIENDLY MOOR — broad area, full mix of fauna at equal weights
            // -----------------------------------------------------------------
            RegionalPools[Region.FriendlyMoor] = new EncounterPool("FriendlyMoor")
                .Add(AddressableAccess.Creatures.Frog_Melee,    minLevel: 1, maxLevel: 18, weight: 1f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Frog_Ranged,             minLevel: 1, maxLevel: 15, weight: 1f, baseCount: 2, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Bloodquito_Rotmoor,             minLevel: 1, maxLevel: 15, weight: 1f, baseCount: 2, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Harpy,             minLevel: 25, maxLevel: 40, weight: 1f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Foglet_Weak,             minLevel: 5, maxLevel: 40, weight: 1f, baseCount: 2, countGrowth: 4)
                .Add(AddressableAccess.Creatures.Foglet,             minLevel: 20, maxLevel: 40, weight: 1f, baseCount: 2, countGrowth: 4)
                ;
            RegionalPools[Region.FriendlyMoor].MinTypesAtLow = 1;
            RegionalPools[Region.FriendlyMoor].MaxTypesAtHigh = 6;
            RegionalPools[Region.FriendlyMoor].SkipChance = 0.75f;

            // -----------------------------------------------------------------
            // RIVER — water-side ambushes
            // -----------------------------------------------------------------
            RegionalPools[Region.River] = new EncounterPool("River")
                .Add(AddressableAccess.Creatures.WaterAmbusher, minLevel: 25,  maxLevel: 30, weight: 1.5f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.WaterDevil,    minLevel: 20, maxLevel: 35, weight: 1f,   baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.WaterDevil_Red, minLevel: 25, maxLevel: 40, weight: 0.5f, baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.WaterRat,      minLevel: 1,  maxLevel: 12, weight: 1.5f, baseCount: 2, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Frog_Melee,      minLevel: 10,  maxLevel: 20, weight: 1.5f, baseCount: 2, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Frog_Ranged,      minLevel: 10,  maxLevel: 20, weight: 1.5f, baseCount: 2, countGrowth: 2)
                ;
            RegionalPools[Region.River].SkipChance = 0.25f;
            RegionalPools[Region.River].MinTypesAtLow = 1;
            RegionalPools[Region.River].MaxTypesAtHigh = 3;


            // -----------------------------------------------------------------
            // HEIDE / HAIN — open heath / grove
            // -----------------------------------------------------------------
            RegionalPools[Region.Heide] = new EncounterPool("Heide")
                .Add(AddressableAccess.Creatures.Boar,    minLevel: 5, maxLevel: 18, weight: 1.5f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.GreatTusk, minLevel: 28, maxLevel: 40, weight: 0.3f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.ShadowRipper, minLevel: 28, maxLevel: 40, weight: 0.3f, baseCount: 1, countGrowth: 3)
                .Add(AddressableAccess.Creatures.Bear, minLevel: 10, maxLevel: 40, weight: 0.3f, baseCount: 1, countGrowth: 3)
                ;
            RegionalPools[Region.Heide].SkipChance = 0.25f;
            RegionalPools[Region.Heide].MinTypesAtLow = 1;
            RegionalPools[Region.Heide].MaxTypesAtHigh = 5;
            
            RegionalPools[Region.Hain] = new EncounterPool("Hain")
                .Add(AddressableAccess.Creatures.Stalker_Red,    minLevel: 18, maxLevel: 40, weight: 1f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.HungryRipper,   minLevel: 8,  maxLevel: 30, weight: 1f, baseCount: 1, countGrowth: 2)
                ;

            // -----------------------------------------------------------------
            // SCHLUND — endgame abyss
            // -----------------------------------------------------------------
            RegionalPools[Region.Schlund] = new EncounterPool("Schlund")
                .Add(AddressableAccess.Creatures.ShadowRipper,         minLevel: 20, maxLevel: 40, weight: 1.5f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.ShadowRipper_Ambush,  minLevel: 25, maxLevel: 40, weight: 1f,   baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Stalker_Red,          minLevel: 25, maxLevel: 40, weight: 1f,   baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.EliteRipper,          minLevel: 30, maxLevel: 40, weight: 0.5f, baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.WaterRat_Young,          minLevel: 1, maxLevel: 10, weight: 0.5f, baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.Bloodquito_Auwald,          minLevel: 1, maxLevel: 10, weight: 0.5f, baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Creatures.WaterRat,          minLevel: 1, maxLevel: 10, weight: 0.5f, baseCount: 1, countGrowth: 1)
                ;

            // -----------------------------------------------------------------
            // CAVE / MINE / RUINS — generic underground
            // -----------------------------------------------------------------
            RegionalPools[Region.Cave] = new EncounterPool("Cave")
                .Add(AddressableAccess.Creatures.BigSpider,      minLevel: 8,  maxLevel: 40, weight: 1.5f, baseCount: 1, countGrowth: 5)
                .Add(AddressableAccess.Creatures.Worm,           minLevel: 15,  maxLevel: 40, weight: 1f,   baseCount: 1, countGrowth: 4)
                .Add(AddressableAccess.Creatures.StoneFist,      minLevel: 8, maxLevel: 40, weight: 0.8f, baseCount: 1, countGrowth: 3)
                ;
            RegionalPools[Region.Cave].SkipChance = 0.25f;
            
            RegionalPools[Region.Mine] = new EncounterPool("Mine")
                .Add(AddressableAccess.Creatures.StoneFist,           minLevel: 12, maxLevel: 35, weight: 1f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.StoneFist_DeepMine,  minLevel: 22, maxLevel: 40, weight: 0.7f, baseCount: 1, countGrowth: 1)
                .Add(AddressableAccess.Bandits.Human_Bandit_Mine_01,  minLevel: 5,  maxLevel: 25, weight: 1.5f, baseCount: 2, countGrowth: 3)
                .Add(AddressableAccess.Bandits.Human_Bandit_Mine_02,  minLevel: 8,  maxLevel: 28, weight: 1.2f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Bandits.Human_Bandit_Mine_03,  minLevel: 14, maxLevel: 35, weight: 1f,   baseCount: 1, countGrowth: 2)
                ;
            RegionalPools[Region.Mine].SkipChance = 0.25f;
            
            RegionalPools[Region.Ruins] = new EncounterPool("Ruins")
                .Add(AddressableAccess.Bandits.Human_Bandit_RuinKasern_01, minLevel: 20,  maxLevel: 40, weight: 2f,   baseCount: 2, countGrowth: 3)
                .Add(AddressableAccess.Bandits.Human_Bandit_RuinKasern_02, minLevel: 20,  maxLevel: 40, weight: 1.5f, baseCount: 2, countGrowth: 3)
                .Add(AddressableAccess.Bandits.Human_Bandit_RuinKasern_03, minLevel: 20, maxLevel: 40, weight: 1f,   baseCount: 1, countGrowth: 2)
                ;
            RegionalPools[Region.Ruins].SkipChance = 0.25f;

            // -----------------------------------------------------------------
            // OVERWORLD — fallback for any region without an explicit pool
            // -----------------------------------------------------------------
            RegionalPools[Region.Overworld_Or_Cave] = new EncounterPool("Overworld")
                .Add(AddressableAccess.Creatures.Boar,    minLevel: 6,  maxLevel: 20, weight: 2f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Snapper, minLevel: 1,  maxLevel: 30, weight: 1f, baseCount: 1, countGrowth: 2)
                .Add(AddressableAccess.Creatures.Spitter, minLevel: 8,  maxLevel: 30, weight: 1f, baseCount: 1, countGrowth: 2)
                ;
            
            // -----------------------------------------------------------------
            // CITY_DUNGEON — fallback for any region without an explicit pool
            // -----------------------------------------------------------------

            RegionalPools[Region.CityDungeon] = new EncounterPool("City Dungeon")
                    .Add(AddressableAccess.Creatures.Draugr_Axe, minLevel: 15, maxLevel: 30, weight: 1.5f, baseCount: 1, countGrowth: 3)
                    .Add(AddressableAccess.Creatures.Draugr_Mage, minLevel: 15, maxLevel: 35, weight: 1f, baseCount: 1, countGrowth: 3)
                    .Add(AddressableAccess.Creatures.Draugr_Spear, minLevel: 15, maxLevel: 32, weight: 1f, baseCount: 1, countGrowth: 3)
                    .Add(AddressableAccess.Creatures.Draugr_Sword_Shield, minLevel: 15, maxLevel: 35, weight: 0.8f, baseCount: 1, countGrowth: 3)
                ;
        }

        private static void BuildGlobalPlaceholders()
        {
            // Wandering wildlife — light pressure
            GlobalPools.Add(new EncounterPool("Wandering Beasts")
                .Add(AddressableAccess.Creatures.Boar,    minLevel: 8,  maxLevel: 20, weight: 2f, baseCount: 1, countGrowth: 4)
                .Add(AddressableAccess.Creatures.Ripper,  minLevel: 5,  maxLevel: 30, weight: 1.5f, baseCount: 1, countGrowth: 8)
                .Add(AddressableAccess.Creatures.Snapper, minLevel: 1,  maxLevel: 30, weight: 1f, baseCount: 1, countGrowth: 5)
                .Add(AddressableAccess.Creatures.Bear,    minLevel: 18, maxLevel: 40, weight: 0.5f, baseCount: 1, countGrowth: 3)
            );

            // Bandit raid — humans
            GlobalPools.Add(new EncounterPool("Bandit Raid")
                .Add(AddressableAccess.Bandits.Human_Bandit_Expedition_01, minLevel: 3,  maxLevel: 20, weight: 1.5f, baseCount: 2, countGrowth: 4)
                .Add(AddressableAccess.Bandits.Human_Bandit_Expedition_02, minLevel: 6,  maxLevel: 25, weight: 1.5f, baseCount: 1, countGrowth: 4)
                .Add(AddressableAccess.Bandits.Human_Bandit_Expedition_03, minLevel: 12, maxLevel: 32, weight: 1f,   baseCount: 1, countGrowth: 4)
                .Add(AddressableAccess.Bandits.Human_Bandit_Expedition_04, minLevel: 18, maxLevel: 40, weight: 0.8f, baseCount: 1, countGrowth: 4)
            );

            // Endgame ambush — saved for high level
            GlobalPools.Add(new EncounterPool("Shadow Hunt")
                .Add(AddressableAccess.Creatures.ShadowRipper,        minLevel: 20, maxLevel: 40, weight: 1.5f, baseCount: 1, countGrowth: 5)
                .Add(AddressableAccess.Creatures.ShadowRipper_Ambush, minLevel: 22, maxLevel: 40, weight: 1f,   baseCount: 1, countGrowth: 5)
                .Add(AddressableAccess.Creatures.EliteRipper,         minLevel: 30, maxLevel: 40, weight: 0.5f, baseCount: 1, countGrowth: 10)
            );
        }
    }
}
