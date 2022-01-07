using UnityEngine;
namespace AssetPackage
{
    internal class AssetBundleLoadAssetRequest<T> : LoadAssetRequest<T>, IAssetBundleRequest where T : Object
    {
        protected AssetBundleRequest bundleRequest;

        public string Name { get; private set; }
        public override bool keepWaiting
        {
            get
            {
                if (Asset)
                    return false;
                if (bundleRequest.isDone)
                {
                    return false;
                }
                return true;
            }
        }

        public AssetBundleLoadAssetRequest(string name)
        {
            Name = name;
        }

        public override float Progeres 
        {
            get
            {
                if (Asset)
                    return 1;
                return bundleRequest == null ? 0 : bundleRequest.progress;
            }
        }

        private void OnBundleLoadFinish(AsyncOperation op)
        {
            SetAsset(bundleRequest.asset as T);
        }

        public void SetRequest(AssetBundleRequest request)
        {
            bundleRequest = request;
            bundleRequest.completed += OnBundleLoadFinish;
        }
    }
}