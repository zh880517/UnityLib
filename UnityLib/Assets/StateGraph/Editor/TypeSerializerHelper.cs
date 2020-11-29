using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class TypeSerializerHelper
{
    private static Dictionary<string, Type> _typeGUIDs;
    public static Dictionary<string, Type> TypeGUIDs
    {
        get
        {
            if (_typeGUIDs == null)
            {
                _typeGUIDs = new Dictionary<string, Type>();
                foreach (var assemble in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in assemble.GetTypes())
                    {
                        if (type.IsInterface || type.IsAbstract)
                            continue;
                        TypeIdentify typeIdentify = type.GetCustomAttribute<TypeIdentify>();
                        if (typeIdentify != null && !string.IsNullOrEmpty(typeIdentify.GUID))
                        {
                            if (_typeGUIDs.TryGetValue(typeIdentify.GUID, out Type exitType))
                            {
                                Debug.LogErrorFormat("类型 {0} 和 {1} 的GUID重复，将被跳过", type.AssemblyQualifiedName, exitType.AssemblyQualifiedName);
                                continue;
                            }
                            else
                            {
                                _typeGUIDs.Add(typeIdentify.GUID, type);
                            }
                        }
                    }
                }
                
            }
            return _typeGUIDs;
        }
    }
    public static SerializationData Serialize(object obj)
    {
        SerializationData elem = new SerializationData
        {
            Type = obj.GetType().AssemblyQualifiedName
        };
        TypeIdentify typeIdentify = obj.GetType().GetCustomAttribute<TypeIdentify>();
        if (typeIdentify != null)
        {
            elem.TypeGUID = typeIdentify.GUID;
        }
#if UNITY_EDITOR
        elem.JsonDatas = UnityEditor.EditorJsonUtility.ToJson(obj);
#else
        elem.JsonDatas = UnityEngine.JsonUtility.ToJson(obj);
#endif

        return elem;
    }

    public static object Deserialize(SerializationData e)
    {
        Type type = null;
        if (string.IsNullOrEmpty(e.TypeGUID) || !TypeGUIDs.TryGetValue(e.TypeGUID, out type))
        {
            if (!string.IsNullOrEmpty(e.Type))
                type = Type.GetType(e.Type);
        }
        if (type == null)
            throw new ArgumentException("Deserializing type is not the same than Json element type");

        var obj = Activator.CreateInstance(type);
#if UNITY_EDITOR
        UnityEditor.EditorJsonUtility.FromJsonOverwrite(e.JsonDatas, obj);
#else
		UnityEngine.JsonUtility.FromJsonOverwrite(e.JsonDatas, obj);
#endif

        return obj;
    }

}
