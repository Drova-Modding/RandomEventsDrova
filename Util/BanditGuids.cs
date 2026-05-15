using System.Reflection;
using Drova_Modding_API.Access;
using UnityEngine.AddressableAssets;

namespace RandomEvents.Util
{
    /// <summary>
    /// Snapshot of every <c>AssetGUID</c> exposed on <see cref="AddressableAccess.Bandits"/>.
    /// Used to decide whether a freshly spawned encounter needs the bandit blackboard fix-up
    /// from <c>BanditSpawningHelper.ModifyBanditPosition</c>.
    /// </summary>
    internal static class BanditGuids
    {
        private static readonly HashSet<string> Guids = BuildGuidSet();

        public static bool IsBandit(AssetReferenceGameObject asset)
        {
            if (asset == null) return false;
            string guid = asset.AssetGUID;
            return !string.IsNullOrEmpty(guid) && Guids.Contains(guid);
        }

        private static HashSet<string> BuildGuidSet()
        {
            var set = new HashSet<string>();
            foreach (var f in typeof(AddressableAccess.Bandits)
                         .GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (f.GetValue(null) is AssetReferenceGameObject asset && !string.IsNullOrEmpty(asset.AssetGUID))
                    set.Add(asset.AssetGUID);
            }
            return set;
        }
    }
}
