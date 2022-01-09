using UnityEngine;
namespace AssetPackage
{
    internal class FileLoadAssetBundleRequest : LoadAssetBundleRequest
    {
        private AssetBundleCreateRequest createRequest;
        public override float Progress => createRequest.progress;

        public override bool IsDone => !createRequest.isDone;

        protected override AssetBundle GetAssetBundle()
        {
            return createRequest.assetBundle;
        }

        public FileLoadAssetBundleRequest(string path)
        {
            createRequest = AssetBundle.LoadFromFileAsync(path);
        }
    }

}
