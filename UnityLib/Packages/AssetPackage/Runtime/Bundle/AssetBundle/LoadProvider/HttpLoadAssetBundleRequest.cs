using UnityEngine;
using UnityEngine.Networking;

namespace AssetPackage
{
    public class HttpLoadAssetBundleRequest : LoadAssetBundleRequest
    {
        private UnityWebRequest webRequest;
        public override bool keepWaiting => !webRequest.isDone;

        public override float Progress => webRequest.downloadProgress;

        public override AssetBundle GetAssetBundle()
        {
            return DownloadHandlerAssetBundle.GetContent(webRequest);
        }

        public HttpLoadAssetBundleRequest(UnityWebRequest request)
        {
            webRequest = request;
        }
    }
}
