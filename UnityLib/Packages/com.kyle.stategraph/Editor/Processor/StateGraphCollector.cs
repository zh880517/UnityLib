using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

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

    public List<string> Names { get; private set; } = new List<string>();

    protected StateGraphCollector()
    {
        StateGraphProcessor.Regist(this);
        ForceUpdate();
    }

    public void ForceUpdate()
    {
        PathGraphMap.Clear();
        string savePath = GetSavePath();
        if (savePath[savePath.Length - 1] == '/')
            savePath = savePath.Substring(0, savePath.Length - 1);
        var guids = AssetDatabase.FindAssets($"t:{typeof(T).FullName}", new string[] { savePath });
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T graph = AssetDatabase.LoadAssetAtPath<T>(path);
            if (graph != null)
            {
                PathGraphMap.Add(path, graph);
            }
        }
        Names = Graphs.Select(it => it.name).ToList();
    }

    public abstract string GetSavePath();

    public void OnCreate(string assetPath)
    {
        if (!PathGraphMap.ContainsKey(assetPath))
        {
            T graph = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (graph != null)
            {
                var exit = PathGraphMap.FirstOrDefault(it => it.Value == graph);
                if (exit.Value != null )
                {
                    PathGraphMap.Remove(exit.Key);
                }
                PathGraphMap.Add(assetPath, graph);
                Names = Graphs.Select(it => it.name).ToList();
            }
        }
    }

    public void OnRemove(string assetPath)
    {
        PathGraphMap.Remove(assetPath);
        Names = Graphs.Select(it => it.name).ToList();
    }

    public Type GraphType()
    {
        return typeof(T);
    }
}
