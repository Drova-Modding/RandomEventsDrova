using UnityEngine.AddressableAssets;

namespace RandomEvents.Encounters
{
    /// <summary>
    /// Weighted, level-gated pool of creatures and/or <see cref="BanditEntry"/> bandits.
    /// Asset-backed creatures are returned by <see cref="Build"/> (fed into EncounterEvent).
    /// BanditCreator bandits are returned by <see cref="BuildBanditEntries"/> and spawned
    /// directly by the event classes.
    /// </summary>
    public class EncounterPool
    {
        private readonly List<CreatureEntry> _entries = new();
        private readonly List<BanditEntry> _banditEntries = new();

        /// <summary>Human readable label, useful for logging.</summary>
        public string Name { get; }

        /// <summary>Minimum number of distinct creature types picked per spawn at level 1.</summary>
        public int MinTypesAtLow { get; set; } = 1;

        /// <summary>Maximum number of distinct creature types picked per spawn at level 40.</summary>
        public int MaxTypesAtHigh { get; set; } = 3;

        /// <summary>Chance that this pool skips spawning when triggered (0.0 to 1.0).</summary>
        public float SkipChance { get; set; } = 0.25f;

        public EncounterPool(string name) { Name = name; }

        public EncounterPool Add(CreatureEntry entry)
        {
            _entries.Add(entry);
            return this;
        }

        public EncounterPool Add(AssetReferenceGameObject asset, int minLevel = 1, int maxLevel = 40, float weight = 1f, int baseCount = 1, int countGrowth = 0)
        {
            _entries.Add(new CreatureEntry(asset, minLevel, maxLevel, weight, baseCount, countGrowth));
            return this;
        }

        public EncounterPool Add(BanditEntry entry)
        {
            _banditEntries.Add(entry);
            return this;
        }

        public bool HasAny => _entries.Count > 0 || _banditEntries.Count > 0;

        /// <summary>
        /// Build a spawn dictionary scaled to the given player level.
        /// Picks 1..N creature types from level-eligible entries by weight, sized by ScaledCount.
        /// </summary>
        public Dictionary<AssetReferenceGameObject, int> Build(int playerLevel)
        {
            var result = new Dictionary<AssetReferenceGameObject, int>();
            var eligible = new List<CreatureEntry>();
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].MatchesLevel(playerLevel)) eligible.Add(_entries[i]);
            }
            if (eligible.Count == 0) return result;

            int typesToPick = ScaleTypeCount(playerLevel);
            if (typesToPick > eligible.Count) typesToPick = eligible.Count;

            for (int i = 0; i < typesToPick; i++)
            {
                CreatureEntry pick = WeightedPickCreature(eligible);
                if (pick == null) break;
                eligible.Remove(pick);
                int count = pick.ScaledCount(playerLevel);
                if (count <= 0) count = 1;
                if (!result.TryAdd(pick.Asset, count))
                    result[pick.Asset] += count;
            }
            return result;
        }

        /// <summary>
        /// Build a list of <see cref="BanditEntry"/> instances to spawn, scaled to the given
        /// player level. Each element also carries a spawn count. The number of distinct types
        /// selected follows the same <see cref="MinTypesAtLow"/>/<see cref="MaxTypesAtHigh"/>
        /// scaling as <see cref="Build"/>.
        /// </summary>
        public List<(BanditEntry Entry, int Count)> BuildBanditEntries(int playerLevel)
        {
            var result = new List<(BanditEntry, int)>();
            var eligible = new List<BanditEntry>();
            for (int i = 0; i < _banditEntries.Count; i++)
            {
                if (_banditEntries[i].MatchesLevel(playerLevel)) eligible.Add(_banditEntries[i]);
            }
            if (eligible.Count == 0) return result;

            int typesToPick = ScaleTypeCount(playerLevel);
            if (typesToPick > eligible.Count) typesToPick = eligible.Count;

            for (int i = 0; i < typesToPick; i++)
            {
                BanditEntry pick = WeightedPickBandit(eligible);
                if (pick == null) break;
                eligible.Remove(pick);
                int count = pick.ScaledCount(playerLevel);
                if (count <= 0) count = 1;
                result.Add((pick, count));
            }
            return result;
        }

        private int ScaleTypeCount(int playerLevel)
        {
            int clamped = playerLevel < 1 ? 1 : (playerLevel > 40 ? 40 : playerLevel);
            float t = (clamped - 1) / 39f;
            return MinTypesAtLow + (int)System.Math.Round((MaxTypesAtHigh - MinTypesAtLow) * t);
        }

        private static CreatureEntry WeightedPickCreature(List<CreatureEntry> entries)
        {
            float total = 0f;
            for (int i = 0; i < entries.Count; i++) total += entries[i].Weight;
            if (total <= 0f) return entries.Count > 0 ? entries[0] : null;
            float roll = UnityEngine.Random.Range(0f, total);
            float acc = 0f;
            for (int i = 0; i < entries.Count; i++)
            {
                acc += entries[i].Weight;
                if (roll <= acc) return entries[i];
            }
            return entries[entries.Count - 1];
        }

        private static BanditEntry WeightedPickBandit(List<BanditEntry> entries)
        {
            float total = 0f;
            for (int i = 0; i < entries.Count; i++) total += entries[i].Weight;
            if (total <= 0f) return entries.Count > 0 ? entries[0] : null;
            float roll = UnityEngine.Random.Range(0f, total);
            float acc = 0f;
            for (int i = 0; i < entries.Count; i++)
            {
                acc += entries[i].Weight;
                if (roll <= acc) return entries[i];
            }
            return entries[entries.Count - 1];
        }
    }
}
