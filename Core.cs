using Drova_Modding_API.Access;
using Drova_Modding_API.GlobalFields;
using Drova_Modding_API.Systems;
using Drova_Modding_API.Systems.Coop;
using Drova_Modding_API.Systems.WorldEvents;
using MelonLoader;
using RandomEvents.Encounters;
using RandomEvents.Events;
using RandomEvents.Util;
using System.Collections;

[assembly: MelonInfo(typeof(RandomEvents.Core), "RandomEvents", "1.1.4", "TrustNoOneElse", null)]
[assembly: MelonGame("Just2D", "Drova")]
[assembly: MelonAdditionalDependencies("Drova_Modding_API")]

namespace RandomEvents
{
    public class Core : MelonMod
    {
        private readonly HashSet<Region> _registeredRegions = [];
        private readonly HashSet<string> _registeredGlobals = [];

        // The events this mod registered, kept so they can be ended directly. The framework only exposes
        // what it currently considers running, and a creature left standing after an event stops being
        // "running" is exactly the case that has to be cleaned up on joining somebody else's world.
        private readonly List<ScaledEncounterEvent> _globalEvents = [];
        private readonly List<ScaledRegionalEvent> _regionalEvents = [];

        public override void OnInitializeMelon()
        {
            OptionMenuAccess.Instance.OnOptionMenuOpen += ModOptions.Build;
            OptionMenuAccess.Instance.OnOptionMenuClose += RegionBlocker.Apply;

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

            if (sceneName == SceneNames.MainMenu)
            {
                ModOptions.RegisterLocalization();
                return;
            }

            if (sceneName != SceneNames.GameplayMain) return;
            MelonCoroutines.Start(ApplyRegionBlocks());

            // Definitions are re-read on every gameplay load, so an edited JSON file costs a trip
            // through the main menu instead of a game restart. Registered events resolve their
            // pool by key, which is what lets the reloaded values reach them.
            EncounterDefinitions.Load();
            RegisterNewPools();
        }

        /// <summary>
        /// Registers an event for every pool that does not have one yet. The API has no
        /// unregister call, so this only ever adds. A pool whose file disappeared keeps its
        /// event, which then ends immediately because it resolves no definition.
        /// </summary>
        private void RegisterNewPools()
        {
            // Global random events — fired by the API's cooldown timer.
            foreach (KeyValuePair<string, EncounterPool> kvp in EncounterDefinitions.GlobalPools)
            {
                if (!kvp.Value.HasAny || !_registeredGlobals.Add(kvp.Key)) continue;
                ScaledEncounterEvent worldEvent = new(kvp.Key);
                _globalEvents.Add(worldEvent);
                WorldEventSystemManager.RegisterWorldEvent(worldEvent);
            }

            // Regional ambushes — fired when the player enters the region.
            foreach (KeyValuePair<Region, EncounterPool> kvp in EncounterDefinitions.RegionalPools)
            {
                if (!kvp.Value.HasAny || !_registeredRegions.Add(kvp.Key)) continue;
                ScaledRegionalEvent regionalEvent = new(kvp.Key);
                _regionalEvents.Add(regionalEvent);
                WorldEventSystemManager.RegisterRegionalEvent(regionalEvent);
            }

            LoggerInstance.Msg($"Registered {_registeredGlobals.Count} global pools and {_registeredRegions.Count} regional pools.");
        }

        private static IEnumerator ApplyRegionBlocks()
        {
            while (WorldEventSystemManager.Instance == null)
                yield return null;

            WorldEventSystemManager.Instance.AddBlockedRegion(Region.Intro);
            RegionBlocker.Apply();
        }
    }
}
