using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleMgr
{
    public static readonly string ManifestFileName = "StreamingAssets";
    public static readonly string CrcFileName = "assetbundle_crc.txt";

    private static bool useCacheLoad = true;
    private static AssetBundleMgr _Instance;
    public static AssetBundleMgr Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new AssetBundleMgr();
            return _Instance;
        }
    }

    private Dictionary<string, AssetBundleInfo> allAssetBundle = new Dictionary<string, AssetBundleInfo>();

    private readonly AssetBundleLoader loader;

    protected AssetBundleMgr()
    {
        loader = new AssetBundleLoader(40);
    }

    public static void SetLoadMode(bool useCache)
    {
        if (useCache == useCacheLoad)
            return;
        if (_Instance != null)
        {
            useCacheLoad = useCache;
            _Instance.loader.UseCacheLoad(useCache);
        }
    }

    public static void Reset()
    {
        if (_Instance != null)
        {
            _Instance.loader.Clear();
        }
    }

    public void Init()
    {
        string manifestFile = PathUtil.GetStreamAssetFilePath(ManifestFileName);
#if UNITY_EDITOR
        //编辑模式下防止没有构建Assetbundle在初始化的时候报错
        if (!System.IO.File.Exists(manifestFile))
            return;
#endif
        var ab = AssetBundle.LoadFromFile(manifestFile);
        var manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        Dictionary<string, uint> crcs = null;
        string json = FileUtil.ReadString(CrcFileName, true);
        if (json != null)
        {
            crcs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, uint>>(json);
            FileUtil.DeleteFile(CrcFileName);
        }
        InitByManifest(manifest, crcs);
        ab.Unload(true);
    }

    public IEnumerator LoadAll(Action<float> onProgress)
    {
        CountYield countYield = new CountYield(allAssetBundle.Count, onProgress);

        yield return loader.LoadYield(allAssetBundle.Values, countYield);
    }

    public void LoadAll(Action<float> onProgress, Action onComplete)
    {
        CountYield countYield = new CountYield(allAssetBundle.Count, onProgress, onComplete);
        loader.Load(allAssetBundle.Values, countYield);
    }

    public AssetBundle GetAssetBundle(string fullName)
    {
        AssetBundleInfo info = null;
        if (allAssetBundle.TryGetValue(fullName, out info))
            return info.Bundle;
        return null;
    }

    public AssetBundleInfo GetAssetBundleInfo(string fullName)
    {
        AssetBundleInfo info = null;
        allAssetBundle.TryGetValue(fullName, out info);
        return info;
    }

    private static readonly List<AssetBundleInfo> infoCache = new List<AssetBundleInfo>();
    public IEnumerator LoadAssetBundle(AssetBundleInfo info)
    {
        infoCache.Clear();
        if (info.Bundle == null)
            infoCache.Add(info);
        foreach (var dep in info.Dependencies)
        {
            if (dep.Bundle == null)
                infoCache.Add(dep);
        }
        if (infoCache.Count > 0)
            yield return loader.LoadYield(infoCache);
    }
#region 同步加载资源，要保证所有的assetbundle已经加载完成
    public T LoadAssetSync<T>(string fullName, string assetName) where T : UnityEngine.Object
    {
        AssetBundleInfo info;
        if (allAssetBundle.TryGetValue(fullName, out info))
        {
            if (info.Bundle != null)
                return info.Bundle.LoadAsset<T>(assetName);
        }
        return null;
    }

    public T[] LoadAllAssetSync<T>(string fullName) where T : UnityEngine.Object
    {
        AssetBundleInfo info;
        if (allAssetBundle.TryGetValue(fullName, out info))
        {
            if (info.Bundle != null)
                return info.Bundle.LoadAllAssets<T>();
        }
        return null;
    }

    public T[] LoadSubAssetsSync<T>(string fullName, string assetName) where T : UnityEngine.Object
    {
        AssetBundleInfo info;
        if (allAssetBundle.TryGetValue(fullName, out info))
        {
            if (info.Bundle != null)
                return info.Bundle.LoadAssetWithSubAssets<T>(assetName);
        }
        return null;
    }

    #endregion

#region 异步资源加载，可以不用提前加载所有的assetbundle
    public IEnumerator LoadAssetAsync<T>(string fullName, LoadSingleAssetResult<T> result) where T : UnityEngine.Object
    {
        result.LoadFinish = false;
        AssetBundleInfo info;
        if (allAssetBundle.TryGetValue(fullName, out info))
        {
            yield return LoadAssetBundle(info);
            if (info.Bundle != null)
            {
                var request = info.Bundle.LoadAssetAsync<T>(result.AssetName);
                yield return result;
                result.OriginalAsset = request.asset as T;
            }
        }
        result.LoadFinish = true;
    }

    public IEnumerator LoadAllAssetAsync<T>(string fullName, LoadAllAssetResult<T> result) where T : UnityEngine.Object
    {
        result.LoadFinish = false;
        AssetBundleInfo info;
        if (allAssetBundle.TryGetValue(fullName, out info))
        {
            yield return LoadAssetBundle(info);
            if (info.Bundle != null)
            {
                var request = info.Bundle.LoadAllAssetsAsync<T>();
                yield return result;
                result.AllAssets = (T[])request.allAssets;
            }
        }
        result.LoadFinish = true;
    }

    public IEnumerator LoadSubAssetsAsync<T>(string fullName, LoadSubAssetResult<T> result) where T : UnityEngine.Object
    {
        result.LoadFinish = false;
        AssetBundleInfo info;
        if (allAssetBundle.TryGetValue(fullName, out info))
        {
            yield return LoadAssetBundle(info);
            if (info.Bundle != null)
            {
                var request = info.Bundle.LoadAssetWithSubAssetsAsync<T>(result.AssetName);
                yield return result;
                result.SubAssets = (T[])request.allAssets;
            }
        }
        result.LoadFinish = true;
    }

#endregion

    private void InitByManifest(AssetBundleManifest manifest, Dictionary<string, uint> crcs)
    {
        Dictionary<string, AssetBundleInfo> tmpAB = new Dictionary<string, AssetBundleInfo>();
        var allABName = manifest.GetAllAssetBundles();
        foreach (var name in allABName)
        {
            AssetBundleInfo abInfo;
            allAssetBundle.TryGetValue(name, out abInfo);
            if (abInfo == null)
                abInfo = new AssetBundleInfo(name);
            else
                allAssetBundle.Remove(name);
            abInfo.Dependencies.Clear();
            abInfo.BeDependend.Clear();
            if (crcs != null)
                crcs.TryGetValue(name, out abInfo.Crc);

            tmpAB.Add(name, abInfo);
        }
        foreach (var kv in tmpAB)
        {
            var newHash = manifest.GetAssetBundleHash(kv.Key);
            if (kv.Value.Hash.isValid && newHash != kv.Value.Hash && kv.Value.Bundle != null)
            {
                kv.Value.UnLoad();
                kv.Value.Hash = newHash;
            }
            var deps = manifest.GetAllDependencies(kv.Key);
            foreach (var dep in deps)
            {
                AssetBundleInfo abInfo;
                if (tmpAB.TryGetValue(dep, out abInfo))
                {
                    abInfo.BeDependend.Add(kv.Value);
                    kv.Value.Dependencies.Add(abInfo);
                }
            }
        }
        //卸载已经在热更后不再使用的assetbundle
        foreach (var kv in allAssetBundle)
        {
            kv.Value.UnLoad();
        }
        allAssetBundle = tmpAB;
    }
    
}
