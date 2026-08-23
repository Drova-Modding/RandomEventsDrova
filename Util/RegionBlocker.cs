using Drova_Modding_API.Systems;
using Drova_Modding_API.Systems.WorldEvents;

namespace RandomEvents.Util
{
    /// <summary>
    /// Keeps the world event system's blocked-region list in sync with <see cref="ModOptions"/>.
    /// A blocked region stops the global event timer through
    /// <see cref="WorldEventSystemManager.IsPlayerInBlockedRegion"/> and is checked again by
    /// <see cref="Events.ScaledRegionalEvent"/> before it spawns, so both event kinds are covered.
    /// </summary>
    internal static class RegionBlocker
    {
        /// <summary>
        /// Regions that stage a boss fight. Most of them are caves as well; the overlap costs
        /// nothing because a region is only blocked once.
        /// </summary>
        private static readonly Region[] BossRegions =
        {
            Region.SpiderDungeon,
            Region.RuinUnder,
            Region.Ruins,
            Region.CityDungeon,
            Region.Library,
            Region.RedTower,
            Region.Schlund,
            Region.Mutter,
            Region.Hain,
            Region.DeathMoor,
        };

        /// <summary>
        /// Regions the API blocks on its own. Dropping one because a toggle went off would
        /// unblock a region this mod never blocked in the first place.
        /// </summary>
        private static readonly HashSet<Region> ApiBlockedByDefault =
        [
            Region.Nemeton,
            Region.EntryNemeton,
            Region.RedTower,
            Region.Tavern,
            Region.Academy,
            Region.WoodCamp,
            Region.Magecamp,
            Region.Ruinexplorer,
            Region.RuinSchmuggler,
            Region.RuinsCamp
        ];

        private static readonly HashSet<Region> Applied = [];
        private static int _appliedToInstanceId;

        /// <summary>
        /// Applies the current option values to the running manager. Safe to call repeatedly and
        /// outside gameplay, where the manager does not exist yet.
        /// </summary>
        internal static void Apply()
        {
            WorldEventSystemManager manager = WorldEventSystemManager.Instance;
            if (manager == null) return;

            // Loading a save builds a new manager with an empty blocked list, so what was tracked
            // against the previous one says nothing about this one.
            if (manager.GetInstanceID() != _appliedToInstanceId)
            {
                Applied.Clear();
                _appliedToInstanceId = manager.GetInstanceID();
            }

            HashSet<Region> wanted = [];
            if (ModOptions.BlockCaves)
            {
                foreach (Region region in Enum.GetValues<Region>())
                {
                    if (region.IsCaveRegion()) wanted.Add(region);
                }
            }
            if (ModOptions.BlockBossRegions)
            {
                foreach (Region region in BossRegions) wanted.Add(region);
            }
            wanted.ExceptWith(ApiBlockedByDefault);

            foreach (Region region in Applied)
            {
                if (!wanted.Contains(region)) manager.RemoveBlockedRegion(region);
            }
            foreach (Region region in wanted)
            {
                if (!Applied.Contains(region)) manager.AddBlockedRegion(region);
            }

            Applied.Clear();
            Applied.UnionWith(wanted);
        }
    }
}
