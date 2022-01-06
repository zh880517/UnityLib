using System.Collections.Generic;
using UnityEngine;

public abstract class AssetPackageSetting : ScriptableObject
{
    public bool Enable = true;
    public abstract IReadOnlyList<string> AssetNames { get; }

    public abstract T GetAsset<T>(string name) where T : Object;
}
