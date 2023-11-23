using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class SdpLiteStructCatalog
{
    private Dictionary<Type, SdpLiteStruct> sdpStructs = new Dictionary<Type, SdpLiteStruct>();
    public IReadOnlyDictionary<Type, SdpLiteStruct> Structs => sdpStructs;

    private SdpLiteStruct AddStruct(Type type)
    {
        var structType = SdpLiteGeneratorUtils.ToType(type);
        if (structType < SdpLiteStructType.BuiltInStruct)
            return null;
        if (sdpStructs.TryGetValue(type, out var sdpStruct))
            return sdpStruct;
        sdpStruct = SdpLiteGeneratorUtils.ToStruct(type);
        sdpStructs.Add(type, sdpStruct);
        var baseType = type.BaseType;
        var currentStruct = sdpStruct;
        while (baseType != null && baseType != typeof(ValueType) && baseType != typeof(object))
        {
            currentStruct.BaseClass = SdpLiteGeneratorUtils.ToStruct(baseType);
            currentStruct = currentStruct.BaseClass;
            baseType = baseType.BaseType;
        }
        foreach (var f in sdpStruct.Fields)
        {
            AddStruct(f.Info.FieldType);
        }
        PolymorphismCheck(sdpStruct);
        return sdpStruct;
    }

    private void PolymorphismCheck(SdpLiteStruct sdpStruct)
    {
        var baseType = sdpStruct.Type.BaseType;
        while (baseType != null && baseType.IsClass && baseType != typeof(object))
        {
            if (baseType.GetCustomAttribute<SpdLitePolymorphismAttribute>() != null)
            {
                if (Structs.ContainsKey(baseType))
                {
                    sdpStruct.PolymorphismBase = baseType;
                }
            }
            baseType = baseType.BaseType;
        }
        if (sdpStruct.PolymorphismBase == null)
        {
            if(sdpStruct.Type.GetCustomAttribute<SpdLitePolymorphismAttribute>() != null)
            {
                sdpStruct.PolymorphismBase = sdpStruct.Type;
            }
        }
    }


    public void CollectType<T>() where T : SdpLiteCatalogAttribute
    {
        foreach (var assemble in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assemble.GetTypes())
            {
                if (type.IsInterface)
                    continue;
                var catalog = type.GetCustomAttribute<T>();
                if (catalog != null)
                {
                    var sdpStruct = AddStruct(type);
                    sdpStruct.GenSerializeFunction = catalog.SerializeFunction;
                }
            }
        }
        HashSet<Type> usedType = new HashSet<Type>();
        foreach (var kv in sdpStructs)
        {
            foreach (var field in kv.Value.Fields)
            {
                if (field.FieldType == SdpLiteStructType.BuiltInStruct)
                {
                    usedType.Add(field.Info.FieldType);
                }
                if (field.FieldType == SdpLiteStructType.Vector
                    && field.ExternType1 == SdpLiteStructType.BuiltInStruct)
                {

                    if (field.Info.FieldType.IsArray)
                    {
                        usedType.Add(field.Info.FieldType.GetElementType());
                    }
                    else
                    {
                        usedType.Add(field.Info.FieldType.GetGenericArguments()[0]);
                    }
                }
                if (field.FieldType == SdpLiteStructType.Map
                    && field.ExternType1 == SdpLiteStructType.BuiltInStruct
                    && field.ExternType2 == SdpLiteStructType.BuiltInStruct)
                {
                    usedType.Add(field.Info.FieldType.GetGenericArguments()[0]);
                    usedType.Add(field.Info.FieldType.GetGenericArguments()[1]);
                }
            }
        }
        foreach (var type in usedType)
        {
            AddStruct(type);
        }
        //剔除空类
        foreach (var kv in Structs)
        {
            if (kv.Value.BaseClass == null)
                continue;
            if (kv.Value.BaseClass != null && kv.Value.BaseClass.IsEmpty())
                kv.Value.BaseClass = null;
        }
        var keys = Structs.Keys.ToList();
        foreach (var key in keys)
        {
            var val = Structs[key];
            if (val.IsEmpty())
            {
                sdpStructs.Remove(key);
            }
        }
    }
}
