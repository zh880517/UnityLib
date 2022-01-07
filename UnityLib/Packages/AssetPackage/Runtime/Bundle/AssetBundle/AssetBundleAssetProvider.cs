using System;
using System.Collections.Generic;
using UnityEngine;
namespace AssetPackage
{
    public class AssetBundleAssetProvider : IAssetProvider
    {
        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public bool HasAsset(string name)
        {
            throw new NotImplementedException();
        }

        public InstantiateAssetRequest<T> InstantiateAssetAsync<T>(string name, Transform paren, bool worldPositionStays) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public BundleLoadRequest LoadAll()
        {
            throw new NotImplementedException();
        }

        public LoadAssetRequest<T> LoadAssetAsync<T>(string name) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public BundleLoadRequest LoadByNameCheck(Func<string, bool> nameCheck)
        {
            throw new NotImplementedException();
        }

        public void OnUnLoadUnUsedAsset()
        {
            throw new NotImplementedException();
        }

        public BundleLoadRequest Refresh()
        {
            throw new NotImplementedException();
        }
    }
}