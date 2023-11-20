using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class GeneratorUtils
{
    static Dictionary<Type, string> BuiltInType = new Dictionary<Type, string>
    {
        {typeof(long), "long" },
        {typeof(ulong), "ulong" },
        {typeof(int), "int" },
        {typeof(uint), "uint" },
        {typeof(short), "short" },
        {typeof(ushort), "ushort" },
        {typeof(byte), "byte" },
        {typeof(sbyte), "sbyte" },
        {typeof(bool), "bool" },
        {typeof(float), "float" },
        {typeof(double), "double" },
        {typeof(string), "string" },
    };

    public static string TypeToName(Type type)
    {
        if (BuiltInType.TryGetValue(type, out string name))
            return name;
        if (type.IsArray)
        {
            return $"{TypeToName(type.GetElementType())}[]";
        }
        if (type.IsGenericType)
        {
            var paramTypes = type.GenericTypeArguments;
            StringBuilder sb = new StringBuilder();
            string fullName = type.FullName;
            sb.Append(fullName.Substring(0, fullName.IndexOf('`')));
            sb.Append('<');
            for (int i = 0; i < paramTypes.Length; ++i)
            {
                if (i > 1)
                    sb.Append(',');
                sb.Append(TypeToName(paramTypes[i]));
            }
            sb.Append('>');
            return sb.ToString();
        }
        string typeName = type.FullName;
        if (typeName.Contains("+"))
            typeName = typeName.Replace("+", ".");

        return typeName;
    }


    //写入文件，如果写入的内容和已经存在的一致就不再写入，防止文件被修改导致Unity重新编译
    public static void WriteToFile(string filePath, string context)
    {
        if (File.Exists(filePath))
        {
            string existContext = File.ReadAllText(filePath, Encoding.UTF8);
            if (existContext == context)
                return;
        }
        else
        {
            string dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        File.WriteAllText(filePath, context, Encoding.UTF8);
    }
}
