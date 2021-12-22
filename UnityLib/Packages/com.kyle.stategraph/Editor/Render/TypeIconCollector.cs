using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class TypeIconCollector
{
    private TypeIconCollector() { }
    private static TypeIconCollector instance;
    private Dictionary<System.Type, Texture> iconCache = new Dictionary<System.Type, Texture>();

    public static Texture Get(System.Type type)
    {
        if (instance == null)
            instance = new TypeIconCollector();
        if (!instance.iconCache.TryGetValue(type, out var icon))
        {
            var typeIcon = type.GetCustomAttribute<TypeIconAttribute>();
            if (typeIcon != null && !string.IsNullOrEmpty(typeIcon.Name))
            {
                icon = EditorGUIUtility.Load(typeIcon.Name) as Texture;
            }
            instance.iconCache.Add(type, icon);
        }
        return icon;
    }
}
