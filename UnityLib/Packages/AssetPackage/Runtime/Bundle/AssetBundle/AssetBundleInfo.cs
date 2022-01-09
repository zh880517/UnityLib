using System.Collections.Generic;
using UnityEngine;
namespace AssetPackage
{
    internal class AssetBundleInfo
    {
        public string Name;
        public Hash128 Hash;
        protected AssetBundle Bundle;
        public List<AssetBundleInfo> Dependencies = new List<AssetBundleInfo>();

        public virtual void OnLoadFinish(AssetBundle bundle)
        {
            Bundle = bundle;
        }

        public void Unload()
        {
            if (Bundle)
                Bundle.Unload(true);
        }
    }
}