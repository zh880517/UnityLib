using UnityEngine;

namespace AssetPackage
{
    public interface IAssetPathProvider
    {
        string GetAssetBundlePath(string bundlName);
        string GetAssetBundleManifestPath();
        string GetPackageManifestPath();
    }

    //如果需要热更新或者改变默认打包位置需要重写这个类
    public class DefaultAssetPathProvider : IAssetPathProvider
    {
        public string GetAssetBundleManifestPath()
        {
            return Application.streamingAssetsPath + "/bundle/bundle";
        }

        public string GetAssetBundlePath(string bundlName)
        {
            return string.Format("{0}/bundle/{1}", Application.streamingAssetsPath, bundlName);
        }

        public string GetPackageManifestPath()
        {
            return Application.streamingAssetsPath + "/bundle/PackageManifest.json";
        }
    }

}