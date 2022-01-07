using System.Collections.Generic;

namespace AssetPackage
{
    public interface IAssetPackageInfoProvider
    {
        Dictionary<string, string> GetAllAssets();
    }

}
