#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace AssetPackage
{
    public class AssetDatabaseAssetProvider : IAssetProvider
    {
        public static IAssetPackageInfoProvider PackageInfo;
        private Dictionary<string, string> Assets = PackageInfo.GetAllAssets();
        
        public void Destroy()
        {
        }

        public bool HasAsset(string name)
        {
            return Assets.ContainsKey(name);
        }

        public InstantiateAssetRequest<T> InstantiateAssetAsync<T>(string name, Transform paren, bool worldPositionStays) where T : UnityEngine.Object
        {
            T asset = null;
            if (Assets.TryGetValue(name, out string path))
            {
                asset = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return new AssetDatabaseInstantiateAssetRequest<T>(asset, paren, worldPositionStays);
        }


        public LoadAssetRequest<T> LoadAssetAsync<T>(string name) where T : UnityEngine.Object
        {

            T asset = null;
            if (Assets.TryGetValue(name, out string path))
            {
                asset = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return new AssetDatabaseLoadAssetRequest<T>(asset);
        }
        public BundleLoadRequest LoadAll()
        {
            return new AssetDatabaseBundleLoadRequet(2);
        }

        public BundleLoadRequest LoadByNameCheck(Func<string, bool> nameCheck)
        {
            return new AssetDatabaseBundleLoadRequet(1);
        }

        public BundleLoadRequest ReloadBundle(IEnumerable<string> bundleName)
        {
            return new AssetDatabaseBundleLoadRequet(1);
        }
    }

}
#endif
