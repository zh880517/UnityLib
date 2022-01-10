using UnityEngine;
using UnityEngine.Networking;

namespace AssetPackage
{
    internal class HttpLoadAssetBundleRequest : LoadAssetBundleRequest
    {
        private UnityWebRequest webRequest;
        public override  bool IsDone => !webRequest.isDone;

        public override float Progress => webRequest.downloadProgress;

        protected override AssetBundle GetAssetBundle()
        {
            return DownloadHandlerAssetBundle.GetContent(webRequest);
        }

        public HttpLoadAssetBundleRequest(string url, AssetBundleInfo info):base(info)
        {
            webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, info.Hash, 0);
            webRequest.SendWebRequest();
        }
    }
}
