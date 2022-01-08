using System.Collections.Generic;
using UnityEditor;
namespace AssetPackage
{
    //依赖资源分包策略
    public interface IDependenciesPacakageProvider
    {
        AssetBundleBuild[] FilesToPackage(IReadOnlyCollection<string> files);
    }
}