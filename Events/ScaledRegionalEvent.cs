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
    /// encounter from the pool is spawned around them. Spawned creatures despawn when the player
    /// leaves the region OR after the safety timeout.
    /// </summary>
    public class ScaledRegionalEvent : ARegionalEvent
    {
        private readonly EncounterPool _pool;
        private readonly float _enterDelaySeconds;
        private readonly float _selfEndSeconds;
        private readonly bool _runParallel;
        private readonly float _skipChance;

        private readonly ActorWorldLocator _locator = new();
        private readonly List<GameObject> _spawned = new();
        private object _delayToken;
        private object _safetyToken;
        private object _despawnToken;

        public ScaledRegionalEvent(
            Region region,
            EncounterPool pool,
            float enterDelaySeconds = 8f,
            float selfEndSeconds = 240f,
            bool runParallel = false)
            : base(region)
        {
            _pool = pool;
            _enterDelaySeconds = enterDelaySeconds;
            _selfEndSeconds = selfEndSeconds;
            _runParallel = runParallel;
            _skipChance = pool.SkipChance;
            _locator.SetMinMaxRange(new Vector2(260f, 450f));
        }

        public override bool CanRunParallel() => _runParallel;

        public override void OnRegionEntered()
        {
            if (_despawnToken != null)
            {
                MelonCoroutines.Stop(_despawnToken);
                _despawnToken = null;
            }
        }

        public override void OnRegionLeft()
        {
            _despawnToken = MelonCoroutines.Start(DelayedDespawn());
        }

        public override void StartEvent()
        {
            base.StartEvent();
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
            if (_despawnToken != null) { MelonCoroutines.Stop(_despawnToken); _despawnToken = null; }
            DespawnAll();
            base.EndEvent();
        }

        private IEnumerator SpawnAfterDelay()
        {
            yield return new WaitForSeconds(_enterDelaySeconds);
            if (!IsRunning) yield break;

            int level = PlayerLevelHelper.GetPlayerLevel();
            Dictionary<AssetReferenceGameObject, int> table = _pool.Build(level);
            if (table.Count == 0)
            {
                MelonLogger.Msg($"[RandomEvents] Regional pool '{_pool.Name}' had nothing eligible for level {level}.");
                yield break;
            }

            if (!PlayerAccess.TryGetPlayer(out Actor player)) yield break;
            Vector3 origin = player.transform.position;

            foreach (var pair in table)
            {
                for (int i = 0; i < pair.Value; i++)
                {
                    Vector2? spot = _locator.GetRandomFreePosition(new Vector2(origin.x, origin.y));
                    if (!spot.HasValue) continue;
                    var go = pair.Key.InstantiateAsync(spot.Value, Quaternion.identity).WaitForCompletion();
                    if (go != null) _spawned.Add(go);
                }
            }
            MelonLogger.Msg($"[RandomEvents] Regional event '{_pool.Name}' spawned {_spawned.Count} creatures at level {level}.");
            _safetyToken = MelonCoroutines.Start(SafetyEnd());
        }

        private IEnumerator SafetyEnd()
        {
            yield return new WaitForSeconds(_selfEndSeconds);
            EndEvent();
        }

        private IEnumerator DelayedDespawn()
        {
            yield return new WaitForSeconds(30f);
            DespawnAll();
            _despawnToken = null;
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
