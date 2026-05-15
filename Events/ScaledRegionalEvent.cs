using System.Collections;
using Drova_Modding_API.Access;
using Drova_Modding_API.Extensions;
using Drova_Modding_API.Systems;
using Drova_Modding_API.Systems.Spawning;
using Drova_Modding_API.Systems.WorldEvents;
using Drova_Modding_API.Systems.WorldEvents.Regional;
using Il2CppDrova;
using MelonLoader;
using RandomEvents.Encounters;
using RandomEvents.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RandomEvents.Events
{
    /// <summary>
    /// Regional ambush. When the player enters the region, after a short delay, a level-scaled
    /// encounter from the pool is spawned around them. After the player leaves, spawned creatures
    /// stick around for a grace period (so a quick re-entry keeps them) and a re-entry cooldown
    /// prevents respawning by walking in and out.
    /// </summary>
    public class ScaledRegionalEvent : ARegionalEvent
    {
        private readonly EncounterPool _pool;
        private readonly float _enterDelaySeconds;
        private readonly float _selfEndSeconds;
        private readonly float _reEntryCooldownSeconds;
        private readonly float _despawnGraceSeconds;
        private readonly bool _runParallel;
        private readonly float _skipChance;

        private readonly ActorWorldLocator _locator = new();
        private readonly List<GameObject> _spawned = new();
        private object _delayToken;
        private object _safetyToken;
        private object _despawnToken;
        private float _lastEndedAtRealtime = float.NegativeInfinity;

        public ScaledRegionalEvent(
            Region region,
            EncounterPool pool,
            float enterDelaySeconds = 8f,
            float selfEndSeconds = 240f,
            float reEntryCooldownSeconds = 600f,
            float despawnGraceSeconds = 30f,
            bool runParallel = false)
            : base(region)
        {
            _pool = pool;
            _enterDelaySeconds = enterDelaySeconds;
            _selfEndSeconds = selfEndSeconds;
            _reEntryCooldownSeconds = reEntryCooldownSeconds;
            _despawnGraceSeconds = despawnGraceSeconds;
            _runParallel = runParallel;
            _skipChance = pool.SkipChance;
            _locator.SetMinMaxRange(new Vector2(260f, 450f));
        }

        public override bool CanRunParallel() => _runParallel;

        public override void OnRegionEntered()
        {
            // Re-entry within grace window: rescue spawned creatures from pending despawn.
            if (_despawnToken != null)
            {
                MelonCoroutines.Stop(_despawnToken);
                _despawnToken = null;
            }
        }

        public override void OnRegionLeft()
        {
            // No-op. Framework calls EndEvent right after this; despawn is scheduled there.
        }

        public override void StartEvent()
        {
            base.StartEvent();

            // Player came back while previous creatures are still alive (despawn was pending).
            // Keep them, refresh safety, don't respawn.
            if (HasLiveSpawned())
            {
                if (_despawnToken != null) { MelonCoroutines.Stop(_despawnToken); _despawnToken = null; }
                if (_safetyToken != null) MelonCoroutines.Stop(_safetyToken);
                _safetyToken = MelonCoroutines.Start(SafetyEnd());
                return;
            }

            // Re-entry cooldown gate.
            if (Time.realtimeSinceStartup - _lastEndedAtRealtime < _reEntryCooldownSeconds)
            {
                MelonLogger.Msg($"[RandomEvents] Regional event '{_pool.Name}' on cooldown — skipping.");
                EndEvent();
                return;
            }

            if (_skipChance > 0f && UnityEngine.Random.value < _skipChance)
            {
                MelonLogger.Msg($"[RandomEvents] Regional event '{_pool.Name}' skipped by random chance ({_skipChance:P0}).");
                EndEvent();
                return;
            }
            var instance = WorldEventSystemManager.Instance;
            if (instance != null && (instance.IsPlayerInBlockedRegion() || WorldEventSystemManager.IsPlayerInDialogueOrTeleporting()))
            {
                MelonLogger.Msg($"[RandomEvents] Regional event '{_pool.Name}' skipped due to player state (in blocked region, dead, or teleporting).");
                EndEvent();
                return;
            }
            _delayToken = MelonCoroutines.Start(SpawnAfterDelay());
        }

        public override void EndEvent()
        {
            if (_delayToken != null) { MelonCoroutines.Stop(_delayToken); _delayToken = null; }
            if (_safetyToken != null) { MelonCoroutines.Stop(_safetyToken); _safetyToken = null; }

            // Hand spawned creatures off to delayed-despawn instead of killing immediately,
            // so a quick re-entry can keep them alive.
            if (HasLiveSpawned() && _despawnToken == null)
            {
                _despawnToken = MelonCoroutines.Start(DelayedDespawn());
            }

            _lastEndedAtRealtime = Time.realtimeSinceStartup;
            base.EndEvent();
        }

        private IEnumerator SpawnAfterDelay()
        {
            yield return new WaitForSeconds(_enterDelaySeconds);
            if (!IsRunning) yield break;

            int level = PlayerLevelHelper.GetPlayerLevel();
            var table = _pool.Build(level);
            if (table.Count == 0)
            {
                MelonLogger.Msg($"[RandomEvents] Regional pool '{_pool.Name}' had nothing eligible for level {level}.");
                yield break;
            }

            if (!PlayerAccess.TryGetPlayer(out Actor player)) yield break;
            Vector3 origin = player.transform.position;

            // Anchor the encounter at one locator-picked spot; cluster the rest around it
            // so a group spawns together instead of scattering (and avoids the framework's
            // (0,0) fallback when it can't place subsequent picks).
            Vector2? anchor = null;
            for (int attempt = 0; attempt < 8 && anchor == null; attempt++)
            {
                var pick = _locator.GetRandomFreePosition(new Vector2(origin.x, origin.y));
                if (pick.HasValue && pick.Value != Vector2.zero) anchor = pick.Value;
            }
            if (anchor == null)
            {
                MelonLogger.Msg($"[RandomEvents] Regional event '{_pool.Name}' could not find a valid spawn anchor.");
                yield break;
            }

            foreach (var pair in table)
            {
                for (int i = 0; i < pair.Value; i++)
                {
                    Vector2 spot = _spawned.Count == 0 ? anchor.Value : ClusterAround(anchor.Value);
                    var go = pair.Key.InstantiateAsync(spot, Quaternion.identity).WaitForCompletion();
                    if (go != null)
                    {
                        if (BanditGuids.IsBandit(pair.Key)){
                            MelonLogger.Msg($"[RandomEvents] Modifying bandit position for '{go.name}' at {spot}.");
                            //BanditSpawningHelper.ModifyBanditPosition(go, spot);
                        }
                        _spawned.Add(go);
                    }
                }
            }

            // Spawn BanditCreator bandits (new path — no addressable required).
            var banditEntries = _pool.BuildBanditEntries(level);
            foreach (var (entry, count) in banditEntries)
            {
                for (int i = 0; i < count; i++)
                {
                    Vector2 spot = _spawned.Count == 0 ? anchor.Value : ClusterAround(anchor.Value);
                    var go = entry.Spawn("Bandit", spot);
                    if (go != null) _spawned.Add(go);
                }
            }

            MelonLogger.Msg($"[RandomEvents] Regional event '{_pool.Name}' spawned {_spawned.Count} creatures at level {level}.");
            _safetyToken = MelonCoroutines.Start(SafetyEnd());
        }

        private static Vector2 ClusterAround(Vector2 anchor)
        {
            float angle = UnityEngine.Random.value * Mathf.PI * 2f;
            float dist = UnityEngine.Random.Range(5f, 30f);
            return anchor + new Vector2(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist);
        }

        private IEnumerator SafetyEnd()
        {
            yield return new WaitForSeconds(_selfEndSeconds);
            EndEvent();
        }

        private IEnumerator DelayedDespawn()
        {
            yield return new WaitForSeconds(_despawnGraceSeconds);
            DespawnAll();
            _despawnToken = null;
        }

        private bool HasLiveSpawned()
        {
            for (int i = _spawned.Count - 1; i >= 0; i--)
            {
                if (_spawned[i] == null) _spawned.RemoveAt(i);
            }
            return _spawned.Count > 0;
        }

        private void DespawnAll()
        {
            for (int i = 0; i < _spawned.Count; i++)
            {
                if (_spawned[i] != null) UnityEngine.Object.Destroy(_spawned[i]);
            }
            _spawned.Clear();
        }
    }
}
