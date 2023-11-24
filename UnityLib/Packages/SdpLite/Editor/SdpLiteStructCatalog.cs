using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class SdpLitePolymorphismGroup
{
    public SdpLiteStruct Base;
    public List<SdpLiteStruct> SubClass = new List<SdpLiteStruct>();
}

public class SdpLiteStructCatalog
{
    private readonly Dictionary<Type, SdpLiteStruct> sdpStructs = new Dictionary<Type, SdpLiteStruct>();
    private readonly List<SdpLitePolymorphismGroup> polymorphism = new List<SdpLitePolymorphismGroup>();
    public IReadOnlyDictionary<Type, SdpLiteStruct> Structs => sdpStructs;

    public Type CatalogType { get; private set; }
    public SdpLiteStructCatalog ParentCatalog { get; private set; }

    public string OutPutPath { get; private set; }
    public string NameSpace { get; private set; }
    public SdpLitePackType PackType { get; private set; }
    public string ClassName { get; private set; }


    public SdpLiteStructCatalog(Type catalogType)
    {
        CatalogType = catalogType;
        var instance = Activator.CreateInstance(catalogType) as SdpLiteCatalogAttribute;
        OutPutPath = instance.GenerateRooPath;
        NameSpace = instance.NameSpace;
        PackType = instance.PackType;
        ClassName = catalogType.Name.Replace("Attribute", "");
    }

    public void SetParent(SdpLiteStructCatalog parent)
    {
        ParentCatalog = parent;
    }

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
            //如果父类和当前不在一个分组，就不再继续向上查找
            if (baseType.GetCustomAttribute(CatalogType) == null)
                break;
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

    private SdpLiteStruct FindByType(Type type)
    {
        if (Structs.TryGetValue(type, out var sdpStruct))
            return sdpStruct;
        return ParentCatalog?.FindByType(type);
    }

    public void BuildPolymorphism()
    {
        foreach (var kv in sdpStructs)
        {
            if (kv.Key == kv.Value.PolymorphismBase)
            {
                var children = sdpStructs.Where(s => s.Value.PolymorphismBase == kv.Key).Select(s => s.Value).ToList();
                if (children.Count > 0)
                {
                    SdpLitePolymorphismGroup group = new SdpLitePolymorphismGroup
                    {
                        Base = kv.Value
                    };
                    group.SubClass.AddRange(children);
                    polymorphism.Add(group);
                }
                else
                {
                    //如果没有子类，即使标识了多态，不再生成多态相关的接口
                    kv.Value.PolymorphismBase = null;
                }
            }
        }
    }

    public void UpdateDynamicField()
    {
        foreach (var kv in Structs)
        {
            foreach (var field in kv.Value.Fields)
            {
                if (field.IsDynamic)
                {
                    Type fieldRealType = field.Info.FieldType;
                    switch (field.FieldType)
                    {
                        case SdpLiteStructType.Vector:
                            fieldRealType = field.Extern1;
                            break;
                        case SdpLiteStructType.Map:
                            fieldRealType = field.Extern2;
                            break;
                    }
                    var fieldStruct = FindByType(fieldRealType);
                    if (fieldStruct == null || fieldStruct.PolymorphismBase == null)
                    {
                        field.IsDynamic = false;
                    }
                    else
                    {
                        fieldStruct.NeedDynamicUnPack = true;
                    }
                }
            }
        }
    }


    public void CollectType()
    {
        foreach (var assemble in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assemble.GetTypes())
            {
                if (type.IsInterface)
                    continue;
                var catalog = type.GetCustomAttribute(CatalogType);
                if (catalog != null)
                {
                    var sdpStruct = AddStruct(type);
                    sdpStruct.GenSerializeFunction = type.GetCustomAttribute<SdpLiteSerializeFunctionAttribute>() != null;
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
