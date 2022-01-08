using System.Collections.Generic;

namespace AssetPackage
{
    [System.Serializable]
    public class PackageManifest
    {
        [System.Serializable]
        public class BundleInfo
        {
            //AssetBundle包名，可能会带有XX/
            public string Name;
            public List<string> Assets = new List<string>();
        }
        [System.Serializable]
        public struct AssetInfo
        {
            //提供给逻辑接口的资源名Package/AssetName
            public string Name;
            public int Location;//BundleInde << 16 | AssetIndex
        }

        public List<BundleInfo> Bundles = new List<BundleInfo>();
        public List<AssetInfo> Assets = new List<AssetInfo>();
    }
}