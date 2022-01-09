using UnityEngine;

namespace AssetPackage
{
    internal abstract class LoadAssetBundleRequest
    {
        public abstract float Progress { get; }
        public abstract bool IsDone { get; }
        protected abstract AssetBundle GetAssetBundle();
        public AssetBundleInfo BundleInfo;
        public void OnFinish()
        {
            BundleInfo.OnLoadFinish(GetAssetBundle());
        }
    }
}