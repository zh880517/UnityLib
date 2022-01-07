using UnityEngine;

namespace AssetPackage
{
    public abstract class LoadAssetBundleRequest : CustomYieldInstruction
    {
        public abstract float Progress { get; }
        public abstract AssetBundle GetAssetBundle(); 
    }
}