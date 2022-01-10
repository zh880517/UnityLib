using UnityEngine;

namespace AssetPackage
{
    internal abstract class LoadAssetBundleRequest
    {
        public abstract float Progress { get; }
        public abstract bool IsDone { get; }
        protected abstract AssetBundle GetAssetBundle();
        private AssetBundleInfo bundleInfo;
        public LoadAssetBundleRequest(AssetBundleInfo info)
        {
            bundleInfo = info;
        }
        public void OnFinish()
        {
            bundleInfo.OnLoadFinish(GetAssetBundle());
        }
    }
}