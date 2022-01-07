using UnityEngine;

namespace AssetPackage
{
    public interface IAssetBundleLoadProvider
    {
        LoadAssetBundleRequest Load(string name, Hash128 hash, uint crc);
    }

}