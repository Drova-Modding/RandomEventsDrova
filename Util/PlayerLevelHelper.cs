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
        /// Returns the player's level, or <c>false</c> when it is not readable yet.
        /// Callers should wait then instead of building an encounter for level 1.
        /// </summary>
        /// <param name="level">The player's level, when it can be read.</param>
        /// <returns>False in menus, during a load, and for the first frames of a world.</returns>
        public static bool TryGetPlayerLevel(out int level)
        {
            return PlayerAccess.TryGetLevel(out level);
        }
    }
}
