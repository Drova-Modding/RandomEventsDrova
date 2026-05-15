using Drova_Modding_API.Systems;

namespace RandomEvents.Encounters
{
    /// <summary>
    /// In-memory store for encounter pools. Populated from JSON files at startup
    /// by <see cref="EncounterLoader"/>. See Definitions/ in the repo for the
    /// authoring side and the modder-facing docs in README.md.
    /// </summary>
    public static class EncounterDefinitions
    {
        /// <summary>Region-specific pools. Triggered when the player enters the region.</summary>
        public static readonly Dictionary<Region, EncounterPool> RegionalPools = new();

        /// <summary>Global pools rolled by the random world-event timer regardless of region.</summary>
        public static readonly List<EncounterPool> GlobalPools = [];

        public static void Load()
        {
            RegionalPools.Clear();
            GlobalPools.Clear();
            EncounterLoader.Load(RegionalPools, GlobalPools);
        }
    }
}
