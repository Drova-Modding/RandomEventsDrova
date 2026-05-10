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
        /// Returns the Player Level
        /// </summary>
        /// <returns></returns>
        public static int GetPlayerLevel()
        {
            var stats = PlayerAccess.GetPlayerAttributeStats();
            
            if (stats == null) return 1;

            return stats._level;
        }
    }
}
