using System;
using System.Collections.Generic;
using UnityEngine;
namespace AssetPackage
{
    public class AssetBundleAssetProvider : IAssetProvider
    {
        private Dictionary<string, AssetBundleInfo> Bundles = new Dictionary<string, AssetBundleInfo>();
        private List<MainBundle> MainBundles = new List<MainBundle>();
        private Dictionary<string, int> AssetLocations = new Dictionary<string, int>();
        //缓存，在每次调用Resources.UnloadUnusedAssets前清理
        private Dictionary<string, UnityEngine.Object> AssetCache = new Dictionary<string, UnityEngine.Object>();
        public void Destroy()
        {
            AssetLocations.Clear();
            AssetCache.Clear();
            MainBundles.Clear();
            foreach (var kv in Bundles)
            {
                kv.Value.Unload();
            }
            Bundles.Clear();
        }

        public bool HasAsset(string name)
        {
            return AssetLocations.ContainsKey(name);
        }

        public InstantiateAssetRequest<T> InstantiateAssetAsync<T>(string name, Transform parent, bool worldPositionStays) where T : UnityEngine.Object
        {
            if (AssetCache.TryGetValue(name, out var obj))
            {
                AssetBundleInstantiateAssetRequest<T> request = new AssetBundleInstantiateAssetRequest<T>(obj.name, parent, worldPositionStays);
                request.SetAsset(obj as T);
                return request;
            }
            if (AssetLocations.TryGetValue(name, out int location))
            {
                //TODO:优化同时加载多个相同资源时只进行一次加载
                int bundleIndex = location >> 16;
                int assetIndex = location & 0xFFFF;
                var bundle = MainBundles[bundleIndex];
                var request = bundle.InstantiateAsync<T>(assetIndex, parent, worldPositionStays);
                request.OnComplete += (r) => 
                {
                    if (!AssetCache.ContainsKey(name))
                        AssetCache.Add(name, request.OriginalAsset);
                };
                return request;
            }
            Debug.LogErrorFormat("资源不存在 : {0}", name);
            return null;
        }

        public BundleLoadRequest LoadAll()
        {

            throw new NotImplementedException();
        }

        public LoadAssetRequest<T> LoadAssetAsync<T>(string name) where T : UnityEngine.Object
        {
            if (AssetCache.TryGetValue(name, out var obj))
            {
                AssetBundleLoadAssetRequest<T> request = new AssetBundleLoadAssetRequest<T>(obj.name);
                request.SetAsset(obj as T);
                return request;
            }
            if (AssetLocations.TryGetValue(name, out int location))
            {
                //TODO:优化同时加载多个相同资源时只进行一次加载
                int bundleIndex = location >> 16;
                int assetIndex = location & 0xFFFF;
                var bundle = MainBundles[bundleIndex];
                var request = bundle.LoadAsync<T>(assetIndex);
                request.OnComplete += (r) =>
                {
                    if (!AssetCache.ContainsKey(name))
                        AssetCache.Add(name, request.Asset);
                };
                return request;
            }
            Debug.LogErrorFormat("资源不存在 : {0}", name);
            return null;
        }

        public BundleLoadRequest LoadByNameCheck(Func<string, bool> nameCheck)
        {
            throw new NotImplementedException();
        }

        public void OnUnLoadUnUsedAsset()
        {
            AssetCache.Clear();
        }

        public BundleLoadRequest Refresh()
        {
            //TODO:暂时不处理，PC平台不需要热更新
            throw new NotImplementedException();
        }
    }
}