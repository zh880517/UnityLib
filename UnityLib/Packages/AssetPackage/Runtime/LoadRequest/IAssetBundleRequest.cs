using UnityEngine;

namespace AssetPackage
{
    public interface IAssetBundleRequest
    {
        string Name { get; }
        void SetRequest(AssetBundleRequest request);
    }

}
