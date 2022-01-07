using System.Collections.Generic;
using UnityEngine;

namespace AssetPackage
{
    public interface IAssetProvider
    {
        //预加载所有的AssetBundle
        BundleLoadRequest LoadAll();
        //只加载部分Assetbundle
        BundleLoadRequest LoadByNameCheck(System.Func<string, bool> nameCheck);
        //刷新AssetBundle加载，可以在热更新完成后调用
        BundleLoadRequest Refresh();
        bool HasAsset(string name);
        LoadAssetRequest<T> LoadAssetAsync<T>(string name) where T : Object;
        InstantiateAssetRequest<T> InstantiateAssetAsync<T>(string name, Transform paren, bool worldPositionStays) where T : Object;
        void Destroy();
    }

}
