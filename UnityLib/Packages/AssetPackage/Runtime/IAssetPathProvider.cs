namespace AssetPackage
{
    public interface IAssetPathProvider
    {
        string GetAssetBundlePath(string bundlName);
        string GetAssetBundleManifestPath();
        string GetPackageManifestPath();
    }

}