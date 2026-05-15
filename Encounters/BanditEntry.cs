using Drova_Modding_API.Systems.Spawning.Templates;
using UnityEngine;

namespace RandomEvents.Encounters
{
    /// <summary>
    /// Bandit weapon/style archetype. Maps directly to the factory methods on <c>BanditCreator</c>.
    /// Use <c>Random</c> to let the framework pick a loadout each time.
    /// </summary>
    public enum BanditType
    {
        /// <summary>Pick a random loadout from all available archetypes each spawn.</summary>
        Random,
        Dagger,
        Sword,
        Axe,
        SwordShield,
        Spear,
        SpearShield,
        Bow,
        SpearSlingshot,
        SwordSlingshot
    }

    /// <summary>
    /// One <see cref="BanditCreator"/>-backed spawn entry inside an encounter pool.
    /// Unlike <see cref="CreatureEntry"/> this does not need an addressable asset —
    /// the NPC is constructed at runtime with randomised cosmetics and difficulty-scaled gear.
    /// </summary>
    public class BanditEntry
    {
        /// <summary>Weapon archetype for this bandit slot.</summary>
        public BanditType Type { get; }

        /// <summary>Equipment quality tier (Easy / Normal / Hard).</summary>
        public BanditDifficulty Difficulty { get; }

        /// <summary>Minimum player level at which this entry may appear.</summary>
        public int MinLevel { get; }

        /// <summary>Maximum player level at which this entry may appear (inclusive).</summary>
        public int MaxLevel { get; }

        /// <summary>Selection weight. Higher = more likely. Default 1.</summary>
        public float Weight { get; }

        /// <summary>Base group size at the entry's MinLevel.</summary>
        public int BaseCount { get; }

        /// <summary>Extra count added once player reaches MaxLevel (linearly interpolated).</summary>
        public int CountGrowth { get; }

        public BanditEntry(
            BanditType type,
            BanditDifficulty difficulty,
            int minLevel = 1,
            int maxLevel = 40,
            float weight = 1f,
            int baseCount = 1,
            int countGrowth = 0)
        {
            Type = type;
            Difficulty = difficulty;
            MinLevel = minLevel;
            MaxLevel = maxLevel;
            Weight = weight;
            BaseCount = baseCount;
            CountGrowth = countGrowth;
        }

        /// <inheritdoc cref="CreatureEntry.MatchesLevel"/>
        public bool MatchesLevel(int level) => level >= MinLevel && level <= MaxLevel;

        /// <inheritdoc cref="CreatureEntry.ScaledCount"/>
        public int ScaledCount(int level)
        {
            if (CountGrowth <= 0 || MaxLevel == MinLevel) return BaseCount;
            int clamped = level < MinLevel ? MinLevel : (level > MaxLevel ? MaxLevel : level);
            float t = (clamped - MinLevel) / (float)(MaxLevel - MinLevel);
            return BaseCount + (int)System.Math.Round(CountGrowth * t);
        }

        /// <summary>
        /// Spawns a single bandit NPC at <paramref name="position"/> and returns its
        /// <see cref="GameObject"/>. The bandit is automatically hostile to the player.
        /// </summary>
        /// <param name="name">Display name shown in the HUD.</param>
        /// <param name="position">World-space spawn position.</param>
        public GameObject Spawn(string name, Vector2 position)
        {
            return Type switch
            {
                BanditType.Dagger         => BanditCreator.CreateDaggerBandit(name, position, Difficulty),
                BanditType.Sword          => BanditCreator.CreateSwordBandit(name, position, Difficulty),
                BanditType.Axe            => BanditCreator.CreateAxeBandit(name, position, Difficulty),
                BanditType.SwordShield    => BanditCreator.CreateSwordShieldBandit(name, position, Difficulty),
                BanditType.Spear          => BanditCreator.CreateSpearBandit(name, position, Difficulty),
                BanditType.SpearShield    => BanditCreator.CreateSpearShieldBandit(name, position, Difficulty),
                BanditType.Bow            => BanditCreator.CreateBowBandit(name, position, Difficulty),
                BanditType.SpearSlingshot => BanditCreator.CreateSpearSlingshotBandit(name, position, Difficulty),
                BanditType.SwordSlingshot => BanditCreator.CreateSwordSlingshotBandit(name, position, Difficulty),
                _                         => BanditCreator.CreateRandomBandit(name, position, Difficulty)
            };
        }
    }
}

