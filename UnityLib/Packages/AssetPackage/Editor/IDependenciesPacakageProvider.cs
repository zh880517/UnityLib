using System.Collections.Generic;
using System.Linq;
using UnityEditor;
namespace AssetPackage
{
    //依赖资源分包策略
    public interface IDependenciesPacakageProvider
    {
        AssetBundleBuild[] FilesToPackage(IReadOnlyCollection<string> files);
    }

    internal class DefaultDependenciesPacakageProvider : IDependenciesPacakageProvider
    {
        public AssetBundleBuild[] FilesToPackage(IReadOnlyCollection<string> files)
        {
            AssetBundleBuild[] assetBundles = new AssetBundleBuild[1];
            assetBundles[0] = new AssetBundleBuild { assetBundleName = "all_dependencies_package", assetNames = files.ToArray() };
            return assetBundles;
        }
    }
}