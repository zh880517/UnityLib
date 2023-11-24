using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

public struct SdpLiteGenerateStrategy
{
    public ISdpLiteCodeGenerator BuildInPack;
    public ISdpLiteCodeGenerator BuildInUnPack;
    public ISdpLiteCodeGenerator CustomPack;
    public ISdpLiteCodeGenerator CustomUnPack;
}
public static class SdpLiteGeneratorUtils
{
    private static readonly HashSet<Type> IntegerTypes = new HashSet<Type>
    {
        typeof(bool),
        typeof(sbyte), typeof(byte),
        typeof(short), typeof(ushort),
        typeof(int), typeof(uint),
        typeof(long), typeof(ulong),
    };

    private static readonly HashSet<Type> FloatTypes = new HashSet<Type>
    {
        typeof(float), typeof(double),
    };

    private static void Error(string msg)
    {
        UnityEngine.Debug.LogError(msg);
    }

    public static SdpLiteStructType ToType(Type type)
    {
        if (type.IsEnum)
            return SdpLiteStructType.Enum;
        if (type == typeof(string))
            return SdpLiteStructType.String;
        if (IntegerTypes.Contains(type))
            return SdpLiteStructType.Integer;
        if (FloatTypes.Contains(type))
            return SdpLiteStructType.Float;
        if (typeof(IDictionary).IsAssignableFrom(type))
            return SdpLiteStructType.Map;
        if (type.IsArray)
        {
            var elType = ToType(type.GetElementType());
            if (elType == SdpLiteStructType.Vector || elType == SdpLiteStructType.Map)
            {
                Error($"不支持数组嵌套容器, {type.FullName}");
                return SdpLiteStructType.Nonsupport;
            }
            return SdpLiteStructType.Vector;
        }

        if (typeof(IDictionary).IsAssignableFrom(type) && type.GetGenericArguments().Length == 2)
        {
            var elType = ToType(type.GetGenericArguments()[0]);
            var el1Type = ToType(type.GetGenericArguments()[1]);
            if (elType == SdpLiteStructType.Vector || elType == SdpLiteStructType.Map
                || el1Type == SdpLiteStructType.Vector || el1Type == SdpLiteStructType.Map)
            {
                Error($"不支持数据容器类型直接嵌套，可以用struct或者class嵌套一层, {type.FullName}");
                return SdpLiteStructType.Nonsupport;
            }
            return SdpLiteStructType.Map;
        }

        if (typeof(ICollection).IsAssignableFrom(type) && type.GetGenericArguments().Length == 1)
        {
            var elType = ToType(type.GetGenericArguments()[0]);
            if (elType == SdpLiteStructType.Vector || elType == SdpLiteStructType.Map)
            {
                Error($"不支持数据容器类型直接嵌套，可以用struct或者class嵌套一层, {type.FullName}");
                return SdpLiteStructType.Nonsupport;
            }
            return SdpLiteStructType.Vector;
        }

        if (type.IsDefined(typeof(SdpLiteCatalogAttribute), true))
            return SdpLiteStructType.CustomStruct;
        if (!type.IsAbstract)
        {
            //暂时处理为内置类型
            return SdpLiteStructType.BuiltInStruct;
        }
        return SdpLiteStructType.Nonsupport;//暂时不支持多态
    }

    public static SdpLiteFieldInfo ToFiled(FieldInfo info, bool requireIndex)
    {
        var type = ToType(info.FieldType);
        if (type == SdpLiteStructType.Nonsupport)
        {
            Error($"不支持的数据类型, {info.DeclaringType.FullName} - {info.FieldType.Name} {info.Name}");
            return null;
        }
        var attribute = info.GetCustomAttribute<SdpLiteFieldAttribute>();
        if (attribute == null && requireIndex)
            return null;
        if (attribute != null && attribute.FieldIndex <= 0)
        {
            Error($"错误的字段索引, {info.DeclaringType.FullName} - {info.Name}, 索引应该大于等于1");
            return null;
        }
        uint index = attribute != null ? attribute.FieldIndex : 0;

        var field = new SdpLiteFieldInfo
        {
            Index = index,
            Info = info,
            IsDynamic = info.GetCustomAttribute<SdpLiteDynamicFieldAttribute>() !=null,
            FieldType = type,
        };
        if (type == SdpLiteStructType.Vector)
        {
            if (info.FieldType.IsArray)
            {
                field.Extern1 = info.FieldType.GetElementType();
            }
            else
            {
                field.Extern1 = info.FieldType.GetGenericArguments()[0];
            }
        }
        else if (type == SdpLiteStructType.Map)
        {
            var genericArguments = info.FieldType.GetGenericArguments();
            field.Extern1 = genericArguments[0];
            field.Extern2 = genericArguments[1];
        }
        if (field.Extern1 != null)
            field.ExternType1 = ToType(field.Extern1);
        if (field.Extern2 != null)
            field.ExternType1 = ToType(field.Extern2);
        return field;
    }

    public static SdpLiteStruct ToStruct(Type type)
    {
        bool isCustomStruct = type.IsDefined(typeof(SdpLiteCatalogAttribute), true);
        SdpLiteStruct sdpLiteStruct = new SdpLiteStruct { Type = type, IsBuiltIn = !isCustomStruct };
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            //使用BindingFlags过滤有问题，不知道是不是使用方式问题，这里直接取所有的字段，手动判断
            if (!field.IsPublic || field.IsStatic || field.DeclaringType != type)
                continue;
            var sdpField = ToFiled(field, isCustomStruct);
            if (sdpField == null)
                continue;
            sdpLiteStruct.Fields.Add(sdpField);
        }
        if (!isCustomStruct)
        {
            //如果有字段标记了索引，但是类以及基类都没有标记SdpLiteCatalog
            //则作为自定义类型去掉没有标记索引的字段
            if (sdpLiteStruct.Fields.Exists(it => it.Index > 0))
            {
                sdpLiteStruct.Fields.RemoveAll(it => it.Index == 0);
            }
            else
            {
                //如果没有字段标记索引，则按照内置类型处理
                for (int i = 0; i < sdpLiteStruct.Fields.Count; ++i)
                {
                    sdpLiteStruct.Fields[i].Index = (uint)i + 1;
                }
            }
        }
        //这里不处理BaseClass
        return sdpLiteStruct;
    }

    public static string ToParamName(SdpLiteFieldInfo field)
    {
        return $"_{field.Info.Name}";
    }
    public static string ToParamList(SdpLiteStruct sdpStruct, bool withType)
    {
        StringBuilder sb = new StringBuilder();
        if (sdpStruct.BaseClass != null)
        {
            sb.Append(ToParamList(sdpStruct.BaseClass, withType));
        }
        for (int i = 0; i < sdpStruct.Fields.Count; ++i)
        {
            var field = sdpStruct.Fields[i];
            if (withType)
            {
                sb.Append($"{GeneratorUtils.TypeToName(field.Info.FieldType)} ");
            }
            sb.Append($"{ToParamName(field)}");
            if (i < sdpStruct.Fields.Count - 1)
            {
                sb.Append(", ");
            }
        }
        return sb.ToString();
    }


    public static void Generate(SdpLiteStructCatalog catalog)
    {
        var builtInStructs = catalog.Structs.Where(it => it.Value.IsBuiltIn).Select(it => it.Value);
        var customStructs = catalog.Structs.Where(it => !it.Value.IsBuiltIn).Select(it => it.Value);
        var pack = new SdpLitePackGenerator();
        var unpack = new SdpLiteUnPackGenerator();
        var paramPack = new SdpLiteParamPackGenerator();
        string builtInPackFile = Path.Combine(catalog.OutPutPath, $"{catalog.ClassName}Pack_Builtin.cs");
        GeneratorUtils.WriteToFile(builtInPackFile, pack.GenerateCode(builtInStructs, catalog.NameSpace, catalog.ClassName));
        string builtInUnPackFile = Path.Combine(catalog.OutPutPath, $"{catalog.ClassName}UnPack_Builtin.cs");
        GeneratorUtils.WriteToFile(builtInUnPackFile, unpack.GenerateCode(builtInStructs, catalog.NameSpace, catalog.ClassName));

        if ((catalog.PackType & SdpLitePackType.Normal) == SdpLitePackType.Normal)
        {
            string customPackFile = Path.Combine(catalog.OutPutPath, $"{catalog.ClassName}Pack_Custom.cs");
            GeneratorUtils.WriteToFile(customPackFile, pack.GenerateCode(customStructs, catalog.NameSpace, catalog.ClassName));
        }
        if ((catalog.PackType & SdpLitePackType.Param) == SdpLitePackType.Param)
        {
            string customPackFile = Path.Combine(catalog.OutPutPath, $"{catalog.ClassName}ParamPack_Custom.cs");
            GeneratorUtils.WriteToFile(customPackFile, paramPack.GenerateCode(customStructs, catalog.NameSpace, catalog.ClassName));
        }

        string customUnPackFile = Path.Combine(catalog.OutPutPath, $"{catalog.ClassName}UnPack_Custom.cs");
        GeneratorUtils.WriteToFile(customUnPackFile, unpack.GenerateCode(customStructs, catalog.NameSpace, catalog.ClassName));
    }

}