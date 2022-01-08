using System.Collections.Generic;
using UnityEngine;
namespace AssetPackage
{
    internal class AssetBundleInfo
    {
        public string Name;
        public Hash128 Hash;
        public AssetBundle Bundle;
        public List<AssetBundleInfo> Dependencies = new List<AssetBundleInfo>();
    }
}