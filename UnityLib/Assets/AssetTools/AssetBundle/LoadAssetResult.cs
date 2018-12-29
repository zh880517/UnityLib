public class LoadSingleAssetResult<T> where T : UnityEngine.Object
{
    public string AssetName;
    public bool LoadFinish;
    public T OriginalAsset;
}

public class LoadSubAssetResult<T> where T : UnityEngine.Object
{
    public string AssetName;
    public bool LoadFinish;
    public T[] SubAssets;
}

public class LoadAllAssetResult<T> where T : UnityEngine.Object
{
    public bool LoadFinish;
    public T[] AllAssets;
}