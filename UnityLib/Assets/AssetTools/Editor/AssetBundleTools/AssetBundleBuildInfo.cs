using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class AssetBundleBuildInfo
{
    //assetbundle名字，带扩展名, 小写
    public string Name;
    public readonly List<string> Files = new List<string>();

    public T LoadAsset<T>(string name) where T : UnityEngine.Object
    {
        foreach (var file in Files)
        {
            if (Path.GetFileNameWithoutExtension(file) == name)
            {
                return AssetDatabase.LoadAssetAtPath<T>(file);
            }
        }
        return null;
    }

    public List<T> LoadAllAsset<T>() where T : UnityEngine.Object
    {
        List<T> result = new List<T>();
        foreach (var file in Files)
        {
            var objs = AssetDatabase.LoadAllAssetsAtPath(file);
            if (objs != null && objs.Length > 0)
            {
                foreach (var obj in objs)
                {
                    T val = obj as T;
                    if (val != null)
                        result.Add(val);
                }
            }
        }
        return result;
    }

    public List<T> LoadSubAssets<T>(string name) where T : UnityEngine.Object
    {
        List<T> result = new List<T>();
        foreach (var file in Files)
        {
            if (Path.GetFileNameWithoutExtension(file) == name)
            {
                var objs = AssetDatabase.LoadAllAssetsAtPath(file);
                foreach (var obj in objs)
                {
                    T val = obj as T;
                    if (val != null)
                        result.Add(val);
                }
                break;
            }
        }
        return result;
    }
}
