using Drova_Modding_API.Access;
using Drova_Modding_API.UI.Builder;
using Il2Cpp;
using static Il2CppCustomFramework.Localization.LocalizationDB;

namespace RandomEvents.Util
{
    /// <summary>
    /// The mod's rows in the shared "Modding" options panel and the persisted values behind them.
    /// The panel is torn down and rebuilt on every menu open, so <see cref="Build"/> belongs on
    /// <see cref="OptionMenuAccess.OnOptionMenuOpen"/>.
    /// </summary>
    internal static class ModOptions
    {
        internal const string ModName = "RandomEvents";
        internal const string BlockCavesOptionKey = "RandomEvents_BlockCaves";
        internal const string BlockBossRegionsOptionKey = "RandomEvents_BlockBossRegions";
        internal const bool BlockCavesDefault = true;
        internal const bool BlockBossRegionsDefault = true;

        private const string TitleKey = "RandomEvents_Title";
        private const string BlockCavesLabelKey = "RandomEvents_BlockCavesLabel";
        private const string BlockBossRegionsLabelKey = "RandomEvents_BlockBossRegionsLabel";
        private const string OnKey = "RandomEvents_On";
        private const string OffKey = "RandomEvents_Off";

        /// <summary>
        /// Whether encounters are suppressed in caves and dungeons.
        /// Falls back to the default until the options panel has been built once, because that is
        /// what writes the key into the game config.
        /// </summary>
        internal static bool BlockCaves => ReadBool(BlockCavesOptionKey, BlockCavesDefault);

        /// <summary>
        /// Whether encounters are suppressed in the regions that stage a boss fight.
        /// See <see cref="RegionBlocker"/> for which regions those are.
        /// </summary>
        internal static bool BlockBossRegions => ReadBool(BlockBossRegionsOptionKey, BlockBossRegionsDefault);

        /// <summary>
        /// Writes the option labels into the game's .loc files. Run this on the main menu scene,
        /// the localization database does not exist earlier.
        /// </summary>
        internal static void RegisterLocalization()
        {
            LocalizationAccess.CreateLocalizationEntries(new List<LocalizationAccess.LocalizationEntry>
            {
                new(TitleKey, "Random Events", ELanguage.en),
                new(TitleKey, "Zufallsereignisse", ELanguage.de),
                new(TitleKey, "Événements aléatoires", ELanguage.fr),
                new(BlockCavesLabelKey, "Block events in caves and dungeons", ELanguage.en),
                new(BlockCavesLabelKey, "Ereignisse in Höhlen und Dungeons sperren", ELanguage.de),
                new(BlockCavesLabelKey, "Bloquer les événements dans les grottes et donjons", ELanguage.fr),
                new(BlockBossRegionsLabelKey, "Block events in boss areas", ELanguage.en),
                new(BlockBossRegionsLabelKey, "Ereignisse in Bossgebieten sperren", ELanguage.de),
                new(BlockBossRegionsLabelKey, "Bloquer les événements dans les zones de boss", ELanguage.fr),
                new(OnKey, "On", ELanguage.en),
                new(OnKey, "An", ELanguage.de),
                new(OnKey, "Activé", ELanguage.fr),
                new(OffKey, "Off", ELanguage.en),
                new(OffKey, "Aus", ELanguage.de),
                new(OffKey, "Désactivé", ELanguage.fr),
            }, ModName);
        }

        /// <summary>
        /// Adds the mod's rows to the shared "Modding" panel.
        /// A null builder means the rows were already added this open-cycle, which is the API's
        /// dedup guard rather than an error.
        /// </summary>
        internal static void Build()
        {
            OptionUIBuilder builder = OptionMenuAccess.Instance.GetBuilder(ModName);
            if (builder == null) return;

            builder
                .CreateTitle(Text(TitleKey))
                .CreateSwitch(Text(BlockCavesLabelKey), Text(OnKey), Text(OffKey), BlockCavesOptionKey, BlockCavesDefault)
                .CreateSwitch(Text(BlockBossRegionsLabelKey), Text(OnKey), Text(OffKey), BlockBossRegionsOptionKey, BlockBossRegionsDefault)
                .Build();
        }

        private static LocalizedString Text(string key)
        {
            return LocalizationAccess.GetLocalizedString(ModName, key);
        }

        private static bool ReadBool(string key, bool fallback)
        {
            return ConfigAccessor.TryGetConfigValue(key, out bool value) ? value : fallback;
        }
    }
}
