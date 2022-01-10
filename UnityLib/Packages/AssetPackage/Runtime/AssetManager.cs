using UnityEngine;

namespace AssetPackage
{
    public static class AssetManager
    {
        public static IAssetPathProvider PathProvider { get; private set; } = new DefaultAssetPathProvider();
        private static IAssetProvider assetProvider;

        public static InitializeRequest Initialize(bool forceAssetBundle, IAssetPathProvider pathProvider)
        {
            if (pathProvider != null)
                PathProvider = pathProvider;
            if (assetProvider != null)
            {
                if ((forceAssetBundle && assetProvider is AssetDatabaseAssetProvider) || (!forceAssetBundle && assetProvider is AssetBundleAssetProvider))
                {
                    Destroy();
                }
            }
#if UNITY_EDITOR
            //仅编辑器模式下才可以使用AssetDatabase进行加载
            if (assetProvider == null && !forceAssetBundle)
            {
                assetProvider = new AssetDatabaseAssetProvider();
            }
#endif
            if (assetProvider == null)
                assetProvider = new AssetBundleAssetProvider();
            return assetProvider.Initialize();
        }

        public static BundleLoadRequest LoadAll()
        {
            if (assetProvider != null)
                return assetProvider.LoadAll();
            return null;
        }

        public static BundleLoadRequest LoadByNameCheck(System.Func<string, bool> nameCheck)
        {
            if (assetProvider != null)
                return assetProvider.LoadByNameCheck(nameCheck);
            return null;
        }

        public static LoadAssetRequest<T> LoadAssetAsync<T>(string name) where T : Object
        {
            if (assetProvider != null)
                return assetProvider.LoadAssetAsync<T>(name);
            return null;
        }

        public static InstantiateAssetRequest<T> InstantiateAssetAsync<T>(string name, Transform parent, bool worldPositionStays) where T : Object
        {
            if (assetProvider != null)
                return assetProvider.InstantiateAssetAsync<T>(name, parent, worldPositionStays);
            return null;
        }

        public static void Destroy()
        {
            AssetLoadCoroutine.Instance.StopAllCoroutines();
            assetProvider.Destroy();
            assetProvider = null;
        }

        public static void OnUnLoadUnUsedAsset()
        {
            if (assetProvider != null)
                assetProvider.OnUnLoadUnUsedAsset();
        }
    }
}