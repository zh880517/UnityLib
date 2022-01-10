using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace AssetPackage
{
    public class ManifestInitializeRequest : InitializeRequest
    {
        public override bool keepWaiting => !isFinish;
        private bool isFinish;
        private AssetBundleAssetProvider assetProvider;

        public ManifestInitializeRequest(AssetBundleAssetProvider assetProvider)
        {
            this.assetProvider = assetProvider;
        }

        public IEnumerator DoInitialize()
        {
            string packageManifestPath = AssetManager.PathProvider.GetPackageManifestPath();
            string json = null;
            if (packageManifestPath.StartsWith("jar") || packageManifestPath.StartsWith("http"))
            {
                UnityWebRequest webRequest = UnityWebRequest.Get(packageManifestPath);
                webRequest.useHttpContinue = false;
                yield return webRequest.SendWebRequest();
                if (webRequest.result != UnityWebRequest.Result.ConnectionError && webRequest.result != UnityWebRequest.Result.ProtocolError)
                {
                    json = webRequest.downloadHandler.text;
                }
            }
            else
            {
                json = File.ReadAllText(packageManifestPath);
            }
            PackageManifest packageManifest = new PackageManifest();
            if (!string.IsNullOrEmpty(json))
            {
                JsonUtility.FromJsonOverwrite(json, packageManifestPath);
            }
            else
            {
                throw new System.Exception("load PackageManifest fail : " + packageManifestPath);
            }
            string bundleManifestPath = AssetManager.PathProvider.GetAssetBundleManifestPath();
            AssetBundle assetBundle;
            if (bundleManifestPath.StartsWith("jar") || bundleManifestPath.StartsWith("http"))
            {
                UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(bundleManifestPath);
                webRequest.useHttpContinue = false;
                yield return webRequest.SendWebRequest();
                assetBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
            }
            else
            {
                var createRequest = AssetBundle.LoadFromFileAsync(bundleManifestPath);
                yield return createRequest;
                assetBundle = createRequest.assetBundle;
            }
            if (assetBundle == null)
            {
                throw new System.Exception("load PackageManifest fail " + bundleManifestPath);
            }
            var manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            assetProvider.InitByManifest(manifest, packageManifest);
            assetBundle.Unload(true);
            isFinish = true;
            assetProvider = null;
            Finished();
        }
    }
}