using System.Collections.Generic;
using UnityEngine;

namespace AssetPackage
{
    public abstract class AssetPackageSetting : ScriptableObject
    {
        public bool Enable = true;
        public abstract List<string> AssetPaths { get; }

        public abstract bool IsInPackage(Object obj);
    }
}
