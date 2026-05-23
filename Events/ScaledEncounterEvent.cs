using Drova_Modding_API.Systems.Spawning;
using Drova_Modding_API.Systems.WorldEvents;
using MelonLoader;
using RandomEvents.Encounters;
using RandomEvents.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RandomEvents.Events
{
    /// <summary>
    /// Global random world event. On each trigger it picks a fresh, level-scaled encounter
    /// dictionary from its pool so the same event keeps providing variety across the run.
    /// Supports both asset-backed creatures/bandits and <see cref="BanditEntry"/> bandits
    /// created via BanditCreator.
    /// </summary>
    public class ScaledEncounterEvent : EncounterEvent
    {
        private readonly EncounterPool _pool;
        private Vector2? _anchor;
        private readonly ActorWorldLocator _banditLocator = new();
        private readonly SpawnTracker _banditTracker = new();

        public ScaledEncounterEvent(EncounterPool pool, int selfEndInSecond = 180)
            : base(new Dictionary<AssetReferenceGameObject, int>(), selfEndInSecond)
        {
            _pool = pool;
            WorldLocator.SetMinMaxRange(new Vector2(260f, 420f));
            _banditLocator.SetMinMaxRange(new Vector2(260f, 420f));
        }

        public override void StartEvent()
        {

            int level = PlayerLevelHelper.GetPlayerLevel();

            var fresh = _pool.Build(level);
            var banditEntries = _pool.BuildBanditEntries(level);

            if (fresh.Count == 0 && banditEntries.Count == 0)
            {
                MelonLogger.Warning($"[RandomEvents] Pool '{_pool.Name}' had no eligible entries for player level — skipping.");
                WorldEventSystemManager.Instance?.EndEvent();
                return;
            }

            // Asset-backed entries feed the base EncounterEvent mechanism.
            EncountersToSpawn.Clear();
            foreach (var kv in fresh) EncountersToSpawn[kv.Key] = kv.Value;
            _anchor = null;

            // BanditCreator bandits are spawned manually before handing off to the base class,
            // so they land at a valid world position regardless of EncountersToSpawn.
            if (banditEntries.Count > 0)
            {
                SpawnBanditCreatorEntries(banditEntries);
            }
            #if DEBUG
            MelonLogger.Msg($"[RandomEvents] Starting event from pool '{_pool.Name}' — {fresh.Count} asset type(s), {banditEntries.Count} bandit type(s).");
            #endif
            base.StartEvent();
        }

        public override void EndEvent()
        {
            _banditTracker.DespawnAll();
            base.EndEvent();
        }

        private void SpawnBanditCreatorEntries(List<(BanditEntry Entry, int Count)> banditEntries)
        {
            // Find an anchor position relative to the player using the dedicated locator.
            if (!Drova_Modding_API.Access.PlayerAccess.TryGetPlayer(out var player)) return;
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);

            Vector2? anchor = null;
            for (int attempt = 0; attempt < 8 && anchor == null; attempt++)
            {
                var pick = _banditLocator.GetRandomFreePosition(playerPos);
                if (pick.HasValue && pick.Value != Vector2.zero) anchor = pick.Value;
            }

            if (anchor == null)
            {
                MelonLogger.Warning($"[RandomEvents] Pool '{_pool.Name}': could not find a valid spawn position for BanditCreator bandits.");
                return;
            }

            bool first = true;
            foreach (var (entry, count) in banditEntries)
            {
                for (int i = 0; i < count; i++)
                {
                    Vector2 spot = first ? anchor.Value : ClusterAround(anchor.Value);
                    first = false;
                    var lazyActor = entry.Spawn("Bandit", spot);
                    _banditTracker.Add(lazyActor);
                    #if DEBUG
                    MelonLogger.Msg($"[Queued lazy BanditCreator bandit '{lazyActor.GUID}' at {spot}.");
                    #endif
                }
            }
        }

        protected override void OnEncounterSpawned(GameObject spawnedObject, AssetReferenceGameObject assetReference, Vector2 position)
        {
            base.OnEncounterSpawned(spawnedObject, assetReference, position);
            if (spawnedObject == null) return;

            // The first valid framework pick becomes the group anchor; the rest snap into a
            // 5–30 unit ring around it. Also catches the framework's (0,0) fallback when
            // its locator fails on later picks.
            if (!_anchor.HasValue)
            {
                if (position == Vector2.zero) return;
                _anchor = position;
            }
            else
            {
                var finalPos = ClusterAround(_anchor.Value);
                var t = spawnedObject.transform;
                t.position = new Vector3(finalPos.x, finalPos.y, t.position.z);
            }
        }

        private static Vector2 ClusterAround(Vector2 anchor)
        {
            float angle = UnityEngine.Random.value * Mathf.PI * 2f;
            float dist = UnityEngine.Random.Range(5f, 30f);
            return anchor + new Vector2(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist);
        }
    }
}
