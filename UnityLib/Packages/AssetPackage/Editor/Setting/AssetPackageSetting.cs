using System.Collections.Generic;
using UnityEngine;

namespace AssetPackage
{
    //TODO:添加AssetBundle变体支持
    public abstract class AssetPackageSetting : ScriptableObject
    {
        public bool Enable = true;
        public abstract List<string> AssetPaths { get; }

        public abstract bool IsInPackage(string path);
    }
}
