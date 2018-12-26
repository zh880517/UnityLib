using System.Collections.Generic;
using UnityEngine;

public class AssertBundleInfo
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
    public List<AssertBundleInfo> Dependencies = new List<AssertBundleInfo>();
    public List<AssertBundleInfo> BeDependend = new List<AssertBundleInfo>();

    public AssertBundleInfo(string name)
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
