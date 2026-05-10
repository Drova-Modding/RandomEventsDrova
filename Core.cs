using Drova_Modding_API.GlobalFields;
using Drova_Modding_API.Systems.WorldEvents;
using MelonLoader;
using RandomEvents.Encounters;
using RandomEvents.Events;

[assembly: MelonInfo(typeof(RandomEvents.Core), "RandomEvents", "1.0.0", "TrustNoOneElse", null)]
[assembly: MelonGame("Just2D", "Drova")]
[assembly: MelonAdditionalDependencies("Drova_Modding_API")]

namespace RandomEvents
{
    public class Core : MelonMod
    {
        private bool _registered;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            if (sceneName != SceneNames.GameplayMain) return;
            if (_registered) return;

            EncounterDefinitions.BuildPlaceholders();

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
    }
}
