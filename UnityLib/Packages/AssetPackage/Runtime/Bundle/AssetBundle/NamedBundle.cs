using System.Collections.Generic;
using UnityEngine;
namespace AssetPackage
{
    internal class NamedBundle
    {
        public AssetBundle Bundle;
        public List<string> Assets;

        private List<IAssetBundleRequest> Requests = new List<IAssetBundleRequest>();

        public void OnLoadFinish()
        {
            foreach (var request in Requests)
                DoRequest(request);
            Requests.Clear();
        }

        public LoadAssetRequest<T> LoadAsync<T>(int index) where T : Object
        {
            AssetBundleLoadAssetRequest<T> request = new AssetBundleLoadAssetRequest<T>(Assets[index]);
            if (Bundle)
                DoRequest(request);
            else
                Requests.Add(request);

            return request;
        }

        public InstantiateAssetRequest<T> InstantiateAsync<T>(int index, Transform paren = null, bool worldPositionStays = false) where T : Object
        {
            AssetBundleInstantiateAssetRequest<T> request = new AssetBundleInstantiateAssetRequest<T>(Assets[index], paren, worldPositionStays);
            if (Bundle)
                DoRequest(request);
            else
                Requests.Add(request);
            return request;
        }

        private void DoRequest(IAssetBundleRequest request)
        {
           request.SetRequest(Bundle.LoadAssetAsync(request.Name));
        }
    }


}