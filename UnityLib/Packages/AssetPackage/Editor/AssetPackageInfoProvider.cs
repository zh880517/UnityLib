using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AssetPackage
{
    public class AssetPackageInfoProvider : IAssetPackageInfoProvider
    {
        [RuntimeInitializeOnLoadMethod]
        static void onInit()
        {
            AssetDatabaseAssetProvider.PackageInfo = new AssetPackageInfoProvider();
        }
        public Dictionary<string, string> GetAllAssets()
        {
            var instance = AssetPackageBuildSetting.Instance;
            if (instance)
            {
                return instance.GetAssets();
            }
            return new Dictionary<string, string>();
        }
    }
}