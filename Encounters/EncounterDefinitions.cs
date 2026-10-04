using Drova_Modding_API.Systems;

namespace RandomEvents.Encounters
{
    /// <summary>
    /// In-memory store for encounter pools. Populated from JSON files by
    /// <see cref="EncounterLoader"/> on every gameplay scene load. See Definitions/ in the repo
    /// for the authoring side and the modder-facing docs in README.md.
    /// </summary>
    public static class EncounterDefinitions
    {
        /// <summary>Region-specific pools. Triggered when the player enters the region.</summary>
        public static readonly Dictionary<Region, EncounterPool> RegionalPools = new();

        /// <summary>
        /// Global pools rolled by the random world-event timer regardless of region, keyed by
        /// pool name (the definition file name).
        /// </summary>
        public static readonly Dictionary<string, EncounterPool> GlobalPools = new();

        /// <summary>
        /// Re-reads every definition file, replacing what was loaded before. Registered events
        /// resolve their pool through <see cref="GetRegional"/> / <see cref="GetGlobal"/>, so an
        /// edited file takes effect on the next gameplay load without restarting the game.
        /// </summary>
        public static void Load()
        {
            RegionalPools.Clear();
            GlobalPools.Clear();
            EncounterLoader.Load(RegionalPools, GlobalPools);
        }

        /// <summary>
        /// The pool for a region, or <c>null</c> when no definition file is loaded for it.
        /// Look pools up through this instead of holding an instance: <see cref="Load"/> replaces
        /// the entries, and a cached instance would keep serving the values from before the reload.
        /// </summary>
        public static EncounterPool GetRegional(Region region)
        {
            return RegionalPools.GetValueOrDefault(region);
        }

        /// <summary>
        /// The global pool with this name, or <c>null</c> when its definition file is gone.
        /// Same reload caveat as <see cref="GetRegional"/>.
        /// </summary>
        public static EncounterPool GetGlobal(string name)
        {
            return name != null && GlobalPools.TryGetValue(name, out EncounterPool pool) ? pool : null;
        }
    }
}
