using UnityEngine;
namespace AssetPackage
{
    internal class AssetBundleInstantiateAssetRequest<T> : InstantiateAssetRequest<T> where T : Object
    {
        protected AssetBundleRequest bundleRequest;
        public override bool keepWaiting
        {
            get
            {
                if (bundleRequest.isDone)
                {
                    return false;
                }
                return true;
            }
        }
        public AssetBundleInstantiateAssetRequest(AssetBundleRequest request)
        {
            bundleRequest = request;
            bundleRequest.completed += OnBundleLoadFinish;
        }

        public override float Progeres => bundleRequest == null ? 0 : bundleRequest.progress;

        private void OnBundleLoadFinish(AsyncOperation op)
        {

            var OriginalAsset = bundleRequest.asset as T;
            if (OriginalAsset)
                Asset = Object.Instantiate(OriginalAsset, parent, worldPositionStays);
            DoLoadCallBack();
        }
    }
}