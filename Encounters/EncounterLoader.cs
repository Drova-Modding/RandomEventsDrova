using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Drova_Modding_API.Access;
using Drova_Modding_API.Systems;
using MelonLoader;
using UnityEngine.AddressableAssets;

namespace RandomEvents.Encounters
{
    /// <summary>
    /// Loads encounter pools from JSON files next to the mod assembly.
    ///
    /// Expected layout (relative to the .dll location):
    ///   RandomEvents/regions/&lt;RegionName&gt;.json   -> populates RegionalPools[Region.&lt;RegionName&gt;]
    ///   RandomEvents/global/&lt;AnyName&gt;.json       -> appended to GlobalPools
    ///
    /// Each file describes a single EncounterPool. Pool name = filename without extension.
    /// Entries reference creatures/bandits by their field name on
    /// Drova_Modding_API.Access.AddressableAccess.Creatures / .Bandits.
    /// </summary>
    internal static class EncounterLoader
    {
        private const string RootFolderName = "RandomEvents";
        private const string RegionsFolderName = "regions";
        private const string GlobalFolderName = "global";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        public static void Load(Dictionary<Region, EncounterPool> regionalPools, List<EncounterPool> globalPools)
        {
            string root = ResolveRootDir();
            if (root == null || !Directory.Exists(root))
            {
                MelonLogger.Warning($"[RandomEvents] Definition folder '{root}' not found. No encounters loaded.");
                return;
            }

            int regionCount = LoadRegional(Path.Combine(root, RegionsFolderName), regionalPools);
            int globalCount = LoadGlobal(Path.Combine(root, GlobalFolderName), globalPools);
            MelonLogger.Msg($"[RandomEvents] Loaded {regionCount} regional pool(s) and {globalCount} global pool(s) from '{root}'.");
        }

        private static string ResolveRootDir()
        {
            string asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return asmDir == null ? null : Path.Combine(asmDir, RootFolderName);
        }

        private static int LoadRegional(string dir, Dictionary<Region, EncounterPool> regionalPools)
        {
            if (!Directory.Exists(dir)) return 0;
            int count = 0;
            foreach (string path in Directory.GetFiles(dir, "*.json"))
            {
                string regionName = Path.GetFileNameWithoutExtension(path);
                if (!Enum.TryParse(regionName, ignoreCase: false, out Region region))
                {
                    MelonLogger.Warning($"[RandomEvents] '{path}' name '{regionName}' is not a known Region — skipping.");
                    continue;
                }
                EncounterPool pool = ReadPool(path, regionName);
                if (pool != null && pool.HasAny)
                {
                    regionalPools[region] = pool;
                    count++;
                }
            }
            return count;
        }

        private static int LoadGlobal(string dir, List<EncounterPool> globalPools)
        {
            if (!Directory.Exists(dir)) return 0;
            int count = 0;
            foreach (string path in Directory.GetFiles(dir, "*.json"))
            {
                string name = Path.GetFileNameWithoutExtension(path);
                EncounterPool pool = ReadPool(path, name);
                if (pool != null && pool.HasAny)
                {
                    globalPools.Add(pool);
                    count++;
                }
            }
            return count;
        }

        private static EncounterPool ReadPool(string path, string name)
        {
            try
            {
                string text = File.ReadAllText(path);
                PoolDto dto = JsonSerializer.Deserialize<PoolDto>(text, JsonOptions);
                if (dto == null) return null;

                var pool = new EncounterPool(name);
                if (dto.MinTypesAtLow.HasValue) pool.MinTypesAtLow = dto.MinTypesAtLow.Value;
                if (dto.MaxTypesAtHigh.HasValue) pool.MaxTypesAtHigh = dto.MaxTypesAtHigh.Value;
                if (dto.SkipChance.HasValue) pool.SkipChance = dto.SkipChance.Value;

                if (dto.Entries == null) return pool;
                foreach (EntryDto e in dto.Entries)
                {
                    AssetReferenceGameObject asset = Resolve(e);
                    if (asset == null)
                    {
                        MelonLogger.Error($"[RandomEvents] '{path}': could not resolve entry (creature='{e.Creature}', bandit='{e.Bandit}') — skipping.");
                        continue;
                    }
                    pool.Add(asset,
                        minLevel: e.MinLevel ?? 1,
                        maxLevel: e.MaxLevel ?? 40,
                        weight: e.Weight ?? 1f,
                        baseCount: e.BaseCount ?? 1,
                        countGrowth: e.CountGrowth ?? 0);
                }
                return pool;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[RandomEvents] Failed to load '{path}': {ex.Message}");
                return null;
            }
        }

        private static AssetReferenceGameObject Resolve(EntryDto e)
        {
            bool hasCreature = !string.IsNullOrEmpty(e.Creature);
            bool hasBandit = !string.IsNullOrEmpty(e.Bandit);
            if (hasCreature && hasBandit) return null;
            if (hasCreature) return FindField(typeof(AddressableAccess.Creatures), e.Creature);
            if (hasBandit) return FindField(typeof(AddressableAccess.Bandits), e.Bandit);
            return null;
        }

        private static AssetReferenceGameObject FindField(Type host, string fieldName)
        {
            FieldInfo field = host.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
            return field?.GetValue(null) as AssetReferenceGameObject;
        }

        private sealed class PoolDto
        {
            public int? MinTypesAtLow { get; set; }
            public int? MaxTypesAtHigh { get; set; }
            public float? SkipChance { get; set; }
            public List<EntryDto> Entries { get; set; }
        }

        private sealed class EntryDto
        {
            public string Creature { get; set; }
            public string Bandit { get; set; }
            public int? MinLevel { get; set; }
            public int? MaxLevel { get; set; }
            public float? Weight { get; set; }
            public int? BaseCount { get; set; }
            public int? CountGrowth { get; set; }
        }
    }
}
