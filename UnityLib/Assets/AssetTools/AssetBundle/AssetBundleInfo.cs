using System.Collections.Generic;
using UnityEngine;

public class AssetBundleInfo
{
    public enum LoadStage
    {
        None,
        CacheLoading,
        NoneCacheLoading,
        CacheLoaded,
        NoneCacheLoaded,
    }

    public string Name { get; private set; }
    public AssetBundle Bundle;
    public Hash128 Hash;
    public uint Crc;
    public LoadStage Stage;
    public List<AssetBundleInfo> Dependencies = new List<AssetBundleInfo>();
    public List<AssetBundleInfo> BeDependend = new List<AssetBundleInfo>();

    public AssetBundleInfo(string name)
    {
        Name = name;
    }

    public void UnLoad()
    {
        if (Bundle != null)
        {
            Bundle.Unload(false);
            Bundle = null;
        }
    }
}
