using Drova_Modding_API.Access;

namespace RandomEvents.Util
{
    /// <summary>
    /// Reads the player's current level.
    /// </summary>
    public static class PlayerLevelHelper
    {
        /// <summary>
        /// Level which is known the highest in the Drova Commmunity
        /// </summary>
        public const int MaxKnownPlayerLevel = 40;

        /// <summary>
        /// Returns the Player Level, or <c>false</c> when it is not readable yet.
        ///
        /// **The old version answered a failed read with 1**, which is not a missing value but a plausible
        /// one: the stats are unavailable for a frame or two after every world load, and every level-gated
        /// pool would quietly collapse to its level-one entries with nothing anywhere reporting it. A
        /// caller that cannot get a level should wait rather than build an encounter for a beginner.
        /// </summary>
        /// <param name="level">The player's level, when it can be read.</param>
        /// <returns>False in menus, during a load, and for the first frames of a world.</returns>
        public static bool TryGetPlayerLevel(out int level)
        {
            return PlayerAccess.TryGetLevel(out level);
        }
    }
}
