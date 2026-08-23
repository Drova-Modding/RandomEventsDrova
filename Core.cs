using Drova_Modding_API.Access;
using Drova_Modding_API.GlobalFields;
using Drova_Modding_API.Systems;
using Drova_Modding_API.Systems.WorldEvents;
using MelonLoader;
using RandomEvents.Encounters;
using RandomEvents.Events;
using RandomEvents.Util;
using System.Collections;

[assembly: MelonInfo(typeof(RandomEvents.Core), "RandomEvents", "1.1.3", "TrustNoOneElse", null)]
[assembly: MelonGame("Just2D", "Drova")]
[assembly: MelonAdditionalDependencies("Drova_Modding_API")]

namespace RandomEvents
{
    public class Core : MelonMod
    {
        private bool _registered;

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
            if (_registered) return;
            
            EncounterDefinitions.Load();

            // Global random events — fired by the API's cooldown timer.
            foreach (var pool in EncounterDefinitions.GlobalPools)
            {
                if (!pool.HasAny) continue;
                WorldEventSystemManager.RegisterWorldEvent(new ScaledEncounterEvent(pool));
            }

            // Regional ambushes — fired when the player enters the region.
            foreach (var kvp in EncounterDefinitions.RegionalPools)
            {
                if (!kvp.Value.HasAny) continue;
                WorldEventSystemManager.RegisterRegionalEvent(new ScaledRegionalEvent(kvp.Key, kvp.Value));
            }

            _registered = true;
            LoggerInstance.Msg($"Registered {EncounterDefinitions.GlobalPools.Count} global pools and {EncounterDefinitions.RegionalPools.Count} regional pools.");
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
