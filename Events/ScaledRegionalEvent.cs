using System.Collections;
using Drova_Modding_API.Access;
using Drova_Modding_API.Systems;
using Drova_Modding_API.Systems.Spawning;
using Drova_Modding_API.Systems.WorldEvents;
using Drova_Modding_API.Systems.WorldEvents.Regional;
using Il2CppDrova.Utilities.LazyLoading;
using MelonLoader;
using RandomEvents.Encounters;
using RandomEvents.Util;
using UnityEngine;

namespace RandomEvents.Events
{
	/// <summary>
	/// Regional ambush. When the player enters the region, after a short delay, a level-scaled
	/// encounter from the pool is spawned around them. After the player leaves, spawned creatures
	/// stick around for a grace period (so a quick re-entry keeps them), and a re-entry cooldown
	/// prevents respawning by walking in and out.
	/// </summary>
	public class ScaledRegionalEvent : ARegionalEvent
	{
		private readonly float _reEntryCooldownSeconds;
		private readonly bool _runParallel;
		private readonly WaitForSeconds _enterDelayWait;
		private readonly WaitForSeconds _selfEndWait;
		private readonly WaitForSeconds _despawnGraceWait;

		private readonly ActorWorldLocator _locator = new();
		private readonly SpawnTracker _tracker = new();
		private object _delayToken;
		private object _safetyToken;
		private object _despawnToken;
		private float _lastEndedAtRealtime = float.NegativeInfinity;
		private bool _suppressCooldownStamp;

		public ScaledRegionalEvent(
			Region region,
			float enterDelaySeconds = 8f,
			float selfEndSeconds = 240f,
			float reEntryCooldownSeconds = 600f,
			float despawnGraceSeconds = 30f,
			bool runParallel = false)
			: base(region)
		{
			_reEntryCooldownSeconds = reEntryCooldownSeconds;
			_runParallel = runParallel;
			_enterDelayWait = new WaitForSeconds(enterDelaySeconds);
			_selfEndWait = new WaitForSeconds(selfEndSeconds);
			_despawnGraceWait = new WaitForSeconds(despawnGraceSeconds);
			_locator.SetMinMaxRange(new Vector2(260f, 420f));
		}

		/// <summary>
		/// The pool this event spawns from, or <c>null</c> when no definition file is loaded for
		/// the region. Resolved on every trigger rather than captured in the constructor, so a
		/// definition reload reaches an event that was registered on an earlier load.
		/// </summary>
		private EncounterPool Pool => EncounterDefinitions.GetRegional(Region);

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
			if (_tracker.HasLive())
			{
				if (_despawnToken != null)
				{
					MelonCoroutines.Stop(_despawnToken);
					_despawnToken = null;
				}
				if (_safetyToken != null) MelonCoroutines.Stop(_safetyToken);
				_safetyToken = MelonCoroutines.Start(SafetyEnd());
				return;
			}

			EncounterPool pool = Pool;
			if (pool == null)
			{
				EndWithoutCooldown();
				return;
			}

			// Re-entry cooldown gate.
			if (Time.realtimeSinceStartup - _lastEndedAtRealtime < _reEntryCooldownSeconds)
			{
                #if DEBUG
				MelonLogger.Msg($"[RandomEvents] Regional event '{Region}' on cooldown — skipping.");
                #endif
				EndWithoutCooldown();
				return;
			}

			// A skip roll is the pool's own outcome, so it starts a cooldown like a real spawn
			// does. Without that, re-entering until the roll finally passes would defeat skipChance.
			float skipChance = pool.SkipChance;
			if (skipChance > 0f && UnityEngine.Random.value < skipChance)
			{
                #if DEBUG
				MelonLogger.Msg($"[RandomEvents] Regional event '{Region}' skipped by random chance ({skipChance:P0}).");
                #endif
				EndEvent();
				return;
			}
			var instance = WorldEventSystemManager.Instance;
			if (instance != null && (instance.IsPlayerInBlockedRegion() || WorldEventSystemManager.IsPlayerInDialogueOrTeleporting()))
			{
                #if DEBUG
				MelonLogger.Msg($"[RandomEvents] Regional event '{Region}' skipped due to player state (in blocked region, dead, or teleporting).");
  #endif
				EndWithoutCooldown();
				return;
			}
			_delayToken = MelonCoroutines.Start(SpawnAfterDelay());
		}

		public override void EndEvent()
		{
			if (_delayToken != null)
			{
				MelonCoroutines.Stop(_delayToken);
				_delayToken = null;
			}
			if (_safetyToken != null)
			{
				MelonCoroutines.Stop(_safetyToken);
				_safetyToken = null;
			}

			// Hand spawned entities off to delayed-despawn instead of killing immediately,
			// so a quick re-entry can keep them alive.
			if (_tracker.HasLive() && _despawnToken == null)
			{
				_despawnToken = MelonCoroutines.Start(DelayedDespawn());
			}

			if (_suppressCooldownStamp)
			{
				_suppressCooldownStamp = false;
			}
			else
			{
				_lastEndedAtRealtime = Time.realtimeSinceStartup;
			}

			base.EndEvent();
		}

		/// <summary>
		/// Ends the event without moving the cooldown timestamp. Used for the gates that say
		/// nothing about the pool: the cooldown itself, a missing definition, and a player state
		/// the spawn cannot run in. Stamping there would push the next possible spawn a full
		/// cooldown further out every time the player crossed the region border.
		/// </summary>
		private void EndWithoutCooldown()
		{
			_suppressCooldownStamp = true;
			EndEvent();
		}

		private IEnumerator SpawnAfterDelay()
		{
			yield return _enterDelayWait;
			if (!IsRunning) yield break;

			EncounterPool pool = Pool;
			if (pool == null) yield break;

			int level = PlayerLevelHelper.GetPlayerLevel();
			var table = pool.Build(level);
			if (table.Count == 0)
			{
                #if DEBUG
				MelonLogger.Msg($"[RandomEvents] Regional pool '{Region}' had nothing eligible for level {level}.");
                #endif
				yield break;
			}

			if (!PlayerAccess.TryGetPlayer(out var player)) yield break;
			var origin = player.transform.position;

			Vector2? anchor = null;
			for (int attempt = 0; attempt < 8 && anchor == null; attempt++)
			{
				var pick = _locator.GetRandomFreePosition(new Vector2(origin.x, origin.y));
				if (pick.HasValue && pick.Value != Vector2.zero) anchor = pick.Value;
			}
			if (anchor == null)
			{
                #if DEBUG
				MelonLogger.Msg($"[RandomEvents] Regional event '{Region}' could not find a valid spawn anchor.");
                #endif
				yield break;
			}

			foreach (var pair in table)
			{
				for (int i = 0; i < pair.Value; i++)
				{
					Vector2 spot = _tracker.Count == 0 ? anchor.Value : ClusterAround(anchor.Value);
					var go = pair.Key.InstantiateAsync(spot, Quaternion.identity).WaitForCompletion();
					_tracker.Add(go);
				}
			}

			// Spawn BanditCreator bandits as lazy actors (do not force immediate load).
			var banditEntries = pool.BuildBanditEntries(level);
			for (int index = 0; index < banditEntries.Count; index++)
			{
				(var entry, int count) = banditEntries[index];
				for (int i = 0; i < count; i++)
				{
					var spot = _tracker.Count == 0 ? anchor.Value : ClusterAround(anchor.Value);
					_tracker.Add(entry.Spawn("Bandit", spot));
				}
			}

            #if DEBUG
			MelonLogger.Msg($"[RandomEvents] Regional event '{Region}' queued {_tracker.Count} entities at level {level}.");
            #endif

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
			yield return _selfEndWait;
			EndEvent();
		}

		private IEnumerator DelayedDespawn()
		{
			yield return _despawnGraceWait;
			MelonLogger.Msg($"[RandomEvents] Regional event '{Region}' despawning {_tracker.Count} entities after grace period.");
			_tracker.DespawnAll();
			_despawnToken = null;
		}
	}
}
