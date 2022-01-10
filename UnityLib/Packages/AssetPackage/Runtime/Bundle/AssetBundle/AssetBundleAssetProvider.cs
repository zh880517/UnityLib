using System;
using System.Collections.Generic;
using UnityEngine;
namespace AssetPackage
{
    public class AssetBundleAssetProvider : IAssetProvider
    {
        private readonly Dictionary<string, AssetBundleInfo> bundles = new Dictionary<string, AssetBundleInfo>();
        private readonly List<MainBundle> mainBundles = new List<MainBundle>();
        private readonly Dictionary<string, int> assetLocations = new Dictionary<string, int>();
        //缓存，在每次调用Resources.UnloadUnusedAssets前清理
        private readonly Dictionary<string, UnityEngine.Object> assetCache = new Dictionary<string, UnityEngine.Object>();
        private InitializeRequest initializeRequest;
        public void Destroy()
        {
            assetLocations.Clear();
            assetCache.Clear();
            mainBundles.Clear();
            foreach (var kv in bundles)
            {
                kv.Value.Unload();
            }
            bundles.Clear();
            initializeRequest = null;
        }

        public bool HasAsset(string name)
        {
            return assetLocations.ContainsKey(name);
        }

        public InitializeRequest Initialize()
        {
            if (initializeRequest == null)
            {
                initializeRequest = new ManifestInitializeRequest(this);
                AssetLoadCoroutine.Instance.StartCoroutine((initializeRequest as ManifestInitializeRequest).DoInitialize());
            }
            return initializeRequest;
        }

        public InstantiateAssetRequest<T> InstantiateAssetAsync<T>(string name, Transform parent, bool worldPositionStays) where T : UnityEngine.Object
        {
            if (assetCache.TryGetValue(name, out var obj))
            {
                AssetBundleInstantiateAssetRequest<T> request = new AssetBundleInstantiateAssetRequest<T>(obj.name, parent, worldPositionStays);
                request.SetAsset(obj as T);
                return request;
            }
            if (assetLocations.TryGetValue(name, out int location))
            {
                int bundleIndex = location >> 16;
                int assetIndex = location & 0xFFFF;
                var bundle = mainBundles[bundleIndex];
                CheckAndLoadAssetBundle(bundle);
                var request = bundle.InstantiateAsync<T>(assetIndex, parent, worldPositionStays);
                request.OnComplete += (r) => 
                {
                    if (!assetCache.ContainsKey(name))
                        assetCache.Add(name, request.OriginalAsset);
                };
                return request;
            }
            Debug.LogErrorFormat("资源不存在 : {0}", name);
            return null;
        }

        public BundleLoadRequest LoadAll()
        {
            if (assetLocations.Count > 0)
            {
                Queue<AssetBundleInfo> loadQueue = new Queue<AssetBundleInfo>();
                foreach (var kv in bundles)
                {
                    if (!kv.Value.HasLoad)
                    {
                        loadQueue.Enqueue(kv.Value);
                    }
                }
                var request = new AssetBundleBundleLoadRequest(loadQueue);
                AssetLoadCoroutine.Instance.AddLoadTick(request);
                return request;
            }
            return null;
        }

        public LoadAssetRequest<T> LoadAssetAsync<T>(string name) where T : UnityEngine.Object
        {
            if (assetCache.TryGetValue(name, out var obj))
            {
                AssetBundleLoadAssetRequest<T> request = new AssetBundleLoadAssetRequest<T>(obj.name);
                request.SetAsset(obj as T);
                return request;
            }
            if (assetLocations.TryGetValue(name, out int location))
            {
                int bundleIndex = location >> 16;
                int assetIndex = location & 0xFFFF;
                var bundle = mainBundles[bundleIndex];
                CheckAndLoadAssetBundle(bundle);
                var request = bundle.LoadAsync<T>(assetIndex);
                request.OnComplete += (r) =>
                {
                    if (!assetCache.ContainsKey(name))
                        assetCache.Add(name, request.Asset);
                };
                return request;
            }
            Debug.LogErrorFormat("资源不存在 : {0}", name);
            return null;
        }

        public BundleLoadRequest LoadByNameCheck(Func<string, bool> nameCheck)
        {
            if (assetLocations.Count > 0)
            {
                Queue<AssetBundleInfo> loadQueue = new Queue<AssetBundleInfo>();
                foreach (var kv in bundles)
                {
                    if (!kv.Value.HasLoad && nameCheck(kv.Key))
                    {
                        CollectBundleLoad(kv.Value, loadQueue);
                    }
                }
                var request = new AssetBundleBundleLoadRequest(loadQueue);
                AssetLoadCoroutine.Instance.AddLoadTick(request);
                return request;
            }
            return null;
        }

        public void OnUnLoadUnUsedAsset()
        {
            assetCache.Clear();
        }

        public void InitByManifest(AssetBundleManifest assetBundleManifest, PackageManifest packageManifest)
        {
            assetLocations.Clear();
            this.bundles.Clear();
            mainBundles.Clear();
            foreach (var bundle in packageManifest.Bundles)
            {
                var mainBundle = new MainBundle
                {
                    Assets = bundle.Assets,
                    Name = bundle.Name,
                    Hash = assetBundleManifest.GetAssetBundleHash(bundle.Name)
                };

                mainBundles.Add(mainBundle);
                this.bundles.Add(bundle.Name, mainBundle);
            }

            foreach (var asset in packageManifest.Assets)
            {
                assetLocations.Add(asset.Name, asset.Location);
            }
            string[] bundles = assetBundleManifest.GetAllAssetBundles();
            foreach (var name in bundles)
            {
                if (!this.bundles.ContainsKey(name))
                {
                    AssetBundleInfo bundleInfo = new AssetBundleInfo
                    {
                        Name = name,
                        Hash = assetBundleManifest.GetAssetBundleHash(name)
                    };
                    this.bundles.Add(name, bundleInfo);
                }
            }
            foreach (var kv in this.bundles)
            {
                string[] depens = assetBundleManifest.GetAllDependencies(kv.Key);
                foreach (var dep in depens)
                {
                    if (this.bundles.TryGetValue(dep, out var info))
                    {
                        kv.Value.Dependencies.Add(info);
                    }
                }
            }
        }

        private void CheckAndLoadAssetBundle(AssetBundleInfo info)
        {
            if (!info.HasLoad)
            {
                Queue<AssetBundleInfo> loadQueue = new Queue<AssetBundleInfo>();
                CollectBundleLoad(info, loadQueue);
                var request = new AssetBundleBundleLoadRequest(loadQueue);
                AssetLoadCoroutine.Instance.AddLoadTick(request);
            }
        }

        private void CollectBundleLoad(AssetBundleInfo info, Queue<AssetBundleInfo> loadQueue)
        {
            if (!info.HasLoad)
            {
                info.HasLoad = true;
                loadQueue.Enqueue(info);
                foreach (var dep in info.Dependencies)
                {
                    if (!dep.HasLoad)
                    {
                        dep.HasLoad = true;
                        loadQueue.Enqueue(dep);
                    }
                }
            }
        }
    }
}