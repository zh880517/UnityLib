using System.Collections.Generic;
using UnityEngine;

public class AssertBundleManager
{
    public static readonly string ManifestFileName = "StreamingAssets";
    public static readonly string CrcFileName = "assertbundle_crc.txt";

    private static bool useCacheLoad = true;
    private static AssertBundleManager _Instance;
    public static AssertBundleManager Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new AssertBundleManager();
            return _Instance;
        }
    }

    private Dictionary<string, AssertBundleInfo> allAssertBundle = new Dictionary<string, AssertBundleInfo>();

    private readonly AssertBundleLoader loader;

    protected AssertBundleManager()
    {
        loader = new AssertBundleLoader(40);
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
        string manifestFile = PathUtil.GetStreamAssertFilePath(ManifestFileName);
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

    private void InitByManifest(AssetBundleManifest manifest, Dictionary<string, uint> crcs)
    {
        Dictionary<string, AssertBundleInfo> tmpAB = new Dictionary<string, AssertBundleInfo>();
        var allABName = manifest.GetAllAssetBundles();
        foreach (var name in allABName)
        {
            AssertBundleInfo abInfo;
            allAssertBundle.TryGetValue(name, out abInfo);
            if (abInfo == null)
                abInfo = new AssertBundleInfo(name);
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
                AssertBundleInfo abInfo;
                if (tmpAB.TryGetValue(dep, out abInfo))
                {
                    abInfo.BeDependend.Add(kv.Value);
                    kv.Value.Dependencies.Add(abInfo);
                }
            }
        }

        allAssertBundle = tmpAB;
    }
    
}
