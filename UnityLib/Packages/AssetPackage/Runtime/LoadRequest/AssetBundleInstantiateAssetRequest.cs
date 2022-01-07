using UnityEngine;
namespace AssetPackage
{
    internal class AssetBundleInstantiateAssetRequest<T> : InstantiateAssetRequest<T> , IAssetBundleRequest where T : Object
    {
        public string Name { get; private set; }
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

        public override float Progeres => bundleRequest == null ? 0 : bundleRequest.progress;

        public AssetBundleInstantiateAssetRequest(string name, Transform parent, bool worldPositionStays) : base(parent, worldPositionStays)
        {
            Name = name;
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

        protected override T Instantiate()
        {
            return Object.Instantiate(OriginalAsset, parent, worldPositionStays);
        }
    }
}