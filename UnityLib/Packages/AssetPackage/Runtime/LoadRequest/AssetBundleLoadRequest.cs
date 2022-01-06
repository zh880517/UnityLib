using UnityEngine;
namespace AssetPackage
{
    public class AssetBundleLoadRequest<T> : AssetLoadRequest<T> where T : Object
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

        public AssetBundleLoadRequest(AssetBundleRequest request)
        {
            bundleRequest = request;
            bundleRequest.completed += OnBundleLoadFinish;
        }

        public override float Progeres => bundleRequest == null ? 0 : bundleRequest.progress;

        private void OnBundleLoadFinish(AsyncOperation op)
        {
            Asset = bundleRequest.asset as T;
            DoLoadCallBack();
        }
    }
}