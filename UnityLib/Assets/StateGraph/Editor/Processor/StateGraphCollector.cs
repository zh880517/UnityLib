using UnityEditor;
using System.Collections.Generic;
using System;

public interface IStateGraphCollector
{
    void OnRemove(string assetPath);
    void OnCreate(string assetPath);
    string GetSavePath();
    Type GraphType();
}

public abstract class StateGraphCollector<T> : IStateGraphCollector where T : StateGraph
{
    protected Dictionary<string, T> PathGraphMap = new Dictionary<string, T>();

    public IReadOnlyCollection<T> Graphs => PathGraphMap.Values;

    protected StateGraphCollector()
    {
        var guids = AssetDatabase.FindAssets(typeof(T).FullName, new string[]{ GetSavePath()});
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T graph = AssetDatabase.LoadAssetAtPath<T>(path);
            if (graph != null)
            {
                PathGraphMap.Add(path, graph);
            }
        }
        StateGraphProcessor.Regist(this);
    }

    public abstract string GetSavePath();

    public void OnCreate(string assetPath)
    {
        if (!PathGraphMap.ContainsKey(assetPath))
        {
            T graph = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (graph != null)
            {
                PathGraphMap.Add(assetPath, graph);
            }
        }
    }

    public void OnRemove(string assetPath)
    {
        PathGraphMap.Remove(assetPath);
    }

    public Type GraphType()
    {
        return typeof(T);
    }
}
