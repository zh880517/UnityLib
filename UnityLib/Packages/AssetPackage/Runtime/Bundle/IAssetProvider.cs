using UnityEngine;

namespace AssetPackage
{
    public interface IAssetProvider
    {
        InitializeRequest Initialize();
        //预加载所有的AssetBundle
        BundleLoadRequest LoadAll();
        //只加载部分Assetbundle
        BundleLoadRequest LoadByNameCheck(System.Func<string, bool> nameCheck);
        bool HasAsset(string name);
        LoadAssetRequest<T> LoadAssetAsync<T>(string name) where T : Object;
        InstantiateAssetRequest<T> InstantiateAssetAsync<T>(string name, Transform parent, bool worldPositionStays) where T : Object;
        void OnUnLoadUnUsedAsset();
        void Destroy();
    }

}
