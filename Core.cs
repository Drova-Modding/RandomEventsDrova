using Drova_Modding_API.Access;
using Drova_Modding_API.GlobalFields;
using Drova_Modding_API.Systems;
using Drova_Modding_API.Systems.Coop;
using Drova_Modding_API.Systems.WorldEvents;
using MelonLoader;
using RandomEvents.Encounters;
using RandomEvents.Events;
using System.Collections;

[assembly: MelonInfo(typeof(RandomEvents.Core), "RandomEvents", "1.1.2", "TrustNoOneElse")]
[assembly: MelonGame("Just2D", "Drova")]
[assembly: MelonAdditionalDependencies("Drova_Modding_API")]

namespace RandomEvents
{
    public class Core : MelonMod
    {
        private bool _registered;

        // The events this mod registered, kept so they can be ended directly. The framework only exposes
        // what it currently considers running, and a creature left standing after an event stops being
        // "running" is exactly the case that has to be cleaned up on joining somebody else's world.
        private readonly List<ScaledEncounterEvent> _globalEvents = [];
        private readonly List<ScaledRegionalEvent> _regionalEvents = [];

        public override void OnInitializeMelon()
        {
            // Said once, at startup, and it is what gives every ShouldRun check in this mod its meaning.
            // An encounter is world state: it has to be rolled once, by whoever owns the world, or two
            // players get two unrelated ambushes and neither can see the other fighting. With no co-op
            // mod installed the local machine is the authority, so single-player is unaffected.
            CoopAccess.Declare(CoopBehavior.AuthorityGated);

            // **Declaring only covers events that have not started yet, and that is the smaller half.** A
            // session is joined from a world that is already running, so the ordinary way into a shared game
            // is with an ambush in progress: creatures this machine rolled, standing in this machine's world,
            // that the host has never heard of. The gate in StartEvent cannot reach those - they are already
            // past it. So the change is listened for too, and whatever is running is ended the moment this
            // machine stops being the one that decides what the world contains.
            CoopAccess.OnSessionChanged += OnSessionChanged;

            LoggerInstance.Msg("Initialized.");
        }

        /// <summary>
        /// Ends everything this mod is running once the local machine is no longer the world authority.
        ///
        /// Fires on joining and on leaving, and deliberately does nothing in the second direction: becoming
        /// the authority again does not restart anything, because an ambush is rolled for where the player is
        /// standing and a resumed one would put creatures somewhere they left minutes ago.
        /// </summary>
        private void OnSessionChanged()
        {
            try
            {
                if (CoopAccess.ShouldRun(CoopBehavior.AuthorityGated)) return;

                WorldEventSystemManager manager = WorldEventSystemManager.Instance;

                if (manager != null && manager.CurrentEvent != null)
                {
                    // Through the manager rather than the event, so its own CurrentEvent is cleared as well
                    // and the cooldown that would pick the next one starts from here.
                    manager.EndEvent();
                }

                // **Every event this mod registered, not the ones the framework calls running.** An event
                // that has finished spawning and stopped running still has its creatures standing in the
                // world, waiting out a grace period that exists only for a player stepping back into a
                // region. "Which events are running" is a different question from "what did this mod put
                // into this world", and only the second one needs clearing up here.
                for (int index = 0; index < _globalEvents.Count; index++)
                {
                    _globalEvents[index].EndEvent();
                }

                for (int index = 0; index < _regionalEvents.Count; index++)
                {
                    _regionalEvents[index].AbandonForCoop();
                }

                LoggerInstance.Msg($"Cleared {_globalEvents.Count} global and {_regionalEvents.Count} " +
                    "regional event(s): this world belongs to somebody else now, and an ambush rolled here " +
                    "would exist for one player and not the other.");
            }
            catch (Exception e)
            {
                LoggerInstance.Error("Ending running events for a co-op session failed: " + e);
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            if (sceneName != SceneNames.GameplayMain) return;
            MelonCoroutines.Start(DelayBlockIntroRegion());
            if (_registered) return;
            
            EncounterDefinitions.Load();

            // Global random events — fired by the API's cooldown timer.
            foreach (var pool in EncounterDefinitions.GlobalPools)
            {
                if (!pool.HasAny) continue;
                ScaledEncounterEvent worldEvent = new(pool);
                _globalEvents.Add(worldEvent);
                WorldEventSystemManager.RegisterWorldEvent(worldEvent);
            }

            // Regional ambushes — fired when the player enters the region.
            foreach (var kvp in EncounterDefinitions.RegionalPools)
            {
                if (!kvp.Value.HasAny) continue;
                ScaledRegionalEvent regionalEvent = new(kvp.Key, kvp.Value);
                _regionalEvents.Add(regionalEvent);
                WorldEventSystemManager.RegisterRegionalEvent(regionalEvent);
            }

            _registered = true;
            LoggerInstance.Msg($"Registered {EncounterDefinitions.GlobalPools.Count} global pools and {EncounterDefinitions.RegionalPools.Count} regional pools.");
        }

        private static IEnumerator DelayBlockIntroRegion()
        {
            while (WorldEventSystemManager.Instance == null)
                yield return null;

            WorldEventSystemManager.Instance.AddBlockedRegion(Region.Intro);
        }
    }
}
