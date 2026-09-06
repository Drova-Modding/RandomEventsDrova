using Drova_Modding_API.Access;
using Drova_Modding_API.GlobalFields;
using Drova_Modding_API.Systems;
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

        public override void OnInitializeMelon()
        {
            OptionMenuAccess.Instance.OnOptionMenuOpen += ModOptions.Build;
            OptionMenuAccess.Instance.OnOptionMenuClose += RegionBlocker.Apply;
            LoggerInstance.Msg("Initialized.");
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
                WorldEventSystemManager.RegisterWorldEvent(new ScaledEncounterEvent(kvp.Key));
            }

            // Regional ambushes — fired when the player enters the region.
            foreach (KeyValuePair<Region, EncounterPool> kvp in EncounterDefinitions.RegionalPools)
            {
                if (!kvp.Value.HasAny || !_registeredRegions.Add(kvp.Key)) continue;
                WorldEventSystemManager.RegisterRegionalEvent(new ScaledRegionalEvent(kvp.Key));
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
