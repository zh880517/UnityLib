using System.Collections.Generic;

namespace AssetPackage
{
    [System.Serializable]
    public class PackageManifest
    {
        public class BundleInfo
        {
            public string Name;
            public List<string> Assets = new List<string>();
        }
        public class AssetInfo
        {
            public string Name;
            public short BundleIndex;
            public short AssetIndex;
        }

        public List<BundleInfo> Bundles = new List<BundleInfo>();
        public List<AssetInfo> assets = new List<AssetInfo>();
    }
}