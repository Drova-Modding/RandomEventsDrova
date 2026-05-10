using Drova_Modding_API.Systems.Spawning;
using System.Collections.Generic;
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
    /// </summary>
    public class ScaledEncounterEvent : EncounterEvent
    {
        private readonly EncounterPool _pool;

        public ScaledEncounterEvent(EncounterPool pool, int selfEndInSecond = 180)
            : base(new Dictionary<AssetReferenceGameObject, int>(), selfEndInSecond)
        {
            _pool = pool;
            WorldLocator.SetMinMaxRange(new Vector2(260f, 450f));
        }

        public override void StartEvent()
        {
            var fresh = _pool.Build(PlayerLevelHelper.GetPlayerLevel());
            if (fresh.Count == 0)
            {
                MelonLogger.Warning($"[RandomEvents] Pool '{_pool.Name}' had no eligible creatures for player level — skipping.");
                WorldEventSystemManager.Instance?.EndEvent();
                return;
            }
            EncountersToSpawn.Clear();
            foreach (var kv in fresh) EncountersToSpawn[kv.Key] = kv.Value;
            MelonLogger.Msg($"[RandomEvents] Starting event from pool '{_pool.Name}' with {fresh.Count} encounters.");
            base.StartEvent();
        }
    }
}
