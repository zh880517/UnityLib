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

        public HttpLoadAssetBundleRequest(string url, Hash128 hash, uint crc = 0)
        {
            webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, hash, crc);
            webRequest.SendWebRequest();
        }
    }
}
