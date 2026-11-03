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

[assembly: MelonInfo(typeof(RandomEvents.Core), "RandomEvents", "1.3.0", "TrustNoOneElse")]
[assembly: MelonGame("Just2D", "Drova")]
[assembly: MelonAdditionalDependencies("Drova_Modding_API")]

namespace RandomEvents
{
    public class Core : MelonMod
    {
        private readonly HashSet<Region> _registeredRegions = [];
        private readonly HashSet<string> _registeredGlobals = [];

        /// <summary>Kept here because the framework lists only running events, but a finished event's creatures still stand in the world.</summary>
        private readonly List<ScaledEncounterEvent> _globalEvents = [];
        private readonly List<ScaledRegionalEvent> _regionalEvents = [];

        public override void OnInitializeMelon()
        {
            OptionMenuAccess.Instance.OnOptionMenuOpen += ModOptions.Build;
            OptionMenuAccess.Instance.OnOptionMenuClose += RegionBlocker.Apply;

            // Only the world owner rolls encounters, or co-op players each get their own unrelated ambush.
            // Without a co-op mod the local machine is the owner.
            CoopAccess.Declare(CoopBehavior.AuthorityGated);

            // Joining a session mid-ambush leaves creatures the host never saw, so they are ended on the switch.
            CoopAccess.OnSessionChanged += OnSessionChanged;

            LoggerInstance.Msg("Initialized.");
        }

        /// <summary>
        /// Ends everything this mod is running once the local machine is no longer the world authority.
        /// Becoming the authority again restarts nothing, since an ambush belongs to where the player stood.
        /// </summary>
        private void OnSessionChanged()
        {
            try
            {
                if (CoopAccess.ShouldRun(CoopBehavior.AuthorityGated)) return;

                WorldEventSystemManager manager = WorldEventSystemManager.Instance;

                if (manager != null && manager.CurrentEvent != null)
                {
                    // Through the manager, so its CurrentEvent clears and the next cooldown starts now.
                    manager.EndEvent();
                }

                // Every registered event, since a finished one keeps its creatures through a re-entry grace period.
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
