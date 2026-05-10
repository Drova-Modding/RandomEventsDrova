using UnityEngine.AddressableAssets;

namespace RandomEvents.Encounters
{
    /// <summary>
    /// Weighted, level-gated pool of creatures. Used to build the spawn dictionary for an EncounterEvent.
    /// </summary>
    public class EncounterPool
    {
        private readonly List<CreatureEntry> _entries = new();

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

        public bool HasAny => _entries.Count > 0;

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
                CreatureEntry pick = WeightedPick(eligible);
                if (pick == null) break;
                eligible.Remove(pick);
                int count = pick.ScaledCount(playerLevel);
                if (count <= 0) count = 1;
                if (result.ContainsKey(pick.Asset))
                    result[pick.Asset] += count;
                else
                    result[pick.Asset] = count;
            }
            return result;
        }

        private int ScaleTypeCount(int playerLevel)
        {
            int clamped = playerLevel < 1 ? 1 : (playerLevel > 40 ? 40 : playerLevel);
            float t = (clamped - 1) / 39f;
            return MinTypesAtLow + (int)System.Math.Round((MaxTypesAtHigh - MinTypesAtLow) * t);
        }

        private static CreatureEntry WeightedPick(List<CreatureEntry> entries)
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
