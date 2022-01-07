using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AssetPackage
{
    public interface IAssetPathProvider
    {
        string GetAssetBundlePath(string bundlName);

    }

}