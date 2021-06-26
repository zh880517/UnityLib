using System.Collections.Generic;
using UnityEditor;

public class StateGraphProcessor : AssetPostprocessor
{
    private static readonly Dictionary<string, IStateGraphCollector> Collectors = new Dictionary<string, IStateGraphCollector>();

    public static void Regist(IStateGraphCollector collector)
    {
        Collectors.Add(collector.GetSavePath(), collector);
    }


    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var asset in importedAssets)
        {
            if (asset.EndsWith(".asset") && AssetDatabase.LoadMainAssetAtPath(asset) is StateGraph graph)
            {
                foreach (var kv in Collectors)
                {
                    if (kv.Value.GraphType() == graph.GetType())
                    {
                        kv.Value.OnCreate(asset);
                        break;
                    }
                }
            }
        }
        foreach (var asset in deletedAssets)
        {
            if (asset.EndsWith(".asset"))
            {
                foreach (var kv in Collectors)
                {
                    if (asset.StartsWith(kv.Key))
                    {
                        kv.Value.OnRemove(asset);
                        break;
                    }
                }
            }
        }
    }
}
