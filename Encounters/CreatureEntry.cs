using UnityEngine.AddressableAssets;

namespace RandomEvents.Encounters
{
    /// <summary>
    /// One spawnable creature entry inside an encounter pool.
    /// </summary>
    public class CreatureEntry
    {
        /// <summary>Addressable for the creature prefab. See AddressableAccess.Creatures / Bandits.</summary>
        public AssetReferenceGameObject Asset { get; }

        /// <summary>Minimum player level at which this creature may appear.</summary>
        public int MinLevel { get; }

        /// <summary>Maximum player level at which this creature should still appear (inclusive). 40 = endgame cap.</summary>
        public int MaxLevel { get; }

        /// <summary>Selection weight. Higher = more likely. Default 1.</summary>
        public float Weight { get; }

        /// <summary>Base group size at the creature's MinLevel.</summary>
        public int BaseCount { get; }

        /// <summary>Extra count added once player reaches MaxKnownPlayerLevel (linear interpolation between).</summary>
        public int CountGrowth { get; }

        public CreatureEntry(
            AssetReferenceGameObject asset,
            int minLevel = 1,
            int maxLevel = 40,
            float weight = 1f,
            int baseCount = 1,
            int countGrowth = 0)
        {
            Asset = asset;
            MinLevel = minLevel;
            MaxLevel = maxLevel;
            Weight = weight;
            BaseCount = baseCount;
            CountGrowth = countGrowth;
        }

        public bool MatchesLevel(int level) => level >= MinLevel && level <= MaxLevel;

        /// <summary>Linearly scales BaseCount -> BaseCount + CountGrowth across [MinLevel, MaxKnownPlayerLevel].</summary>
        public int ScaledCount(int level)
        {
            if (CountGrowth <= 0 || MaxLevel == MinLevel) return BaseCount;
            int clamped = level < MinLevel ? MinLevel : (level > MaxLevel ? MaxLevel : level);
            float t = (clamped - MinLevel) / (float)(MaxLevel - MinLevel);
            return BaseCount + (int)System.Math.Round(CountGrowth * t);
        }
    }
}
