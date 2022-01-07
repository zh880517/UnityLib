using UnityEngine;
namespace AssetPackage
{
    public class AsyncLoadAssetBundleRequest : LoadAssetBundleRequest
    {
        private AssetBundleCreateRequest createRequest;
        public override float Progress => createRequest.progress;

        public override bool keepWaiting => !createRequest.isDone;

        public override AssetBundle GetAssetBundle()
        {
            return createRequest.assetBundle;
        }

        public AsyncLoadAssetBundleRequest(AssetBundleCreateRequest request)
        {
            createRequest = request;
        }
    }

}
