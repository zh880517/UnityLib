using System.Collections.Generic;
using UnityEngine;

public static class EditorResources
{
    private static readonly Dictionary<string, Object> resources = new Dictionary<string, Object>();

    public static T Get<T>(string path) where T : Object
    {
        if (!resources.ContainsKey(path))
        {
            var resource = Resources.Load<T>(path);
            resources.Add(path, resource);
        }

        return resources[path] as T;
    }
}
