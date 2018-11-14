using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetDependency
{
    public string Path { get; private set; }
    public HashSet<string> Dependencies { get; private set; }
    public string DependenciesHash { get; private set; }

    public AssetDependency(string path)
    {
        Path = path;
        Update();
    }

    public AssetDependency()
    {
         
    }

    public bool Update()
    {
        if (!File.Exists(Path))
            return false;
        var hash = AssetDatabase.GetAssetDependencyHash(Path).ToString();
        if (hash != DependenciesHash)
        {
            DependenciesHash = hash;
            var depen = AssetDatabase.GetDependencies(Path);
            Dependencies = new HashSet<string>();
            foreach (var file in depen)
            {
                if (file.EndsWith(".dll") 
                    || file.EndsWith(".cs") 
                    || file.Contains("unity_builtin_extra") 
                    || file.Contains("unity default resources"))
                    continue;
                Dependencies.Add(file);
            }
        }
        return true;
    }
}
