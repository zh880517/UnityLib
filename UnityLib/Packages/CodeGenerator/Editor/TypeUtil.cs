using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator
{
    public static class TypeUtil
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
            if (type.IsGenericType)
            {
                var paramTypes = type.GenericTypeArguments;
                StringBuilder sb = new StringBuilder();
                string fullName = type.FullName;
                sb.Append(fullName.Substring(0, fullName.IndexOf('`')));
                sb.Append('<');
                for (int i=0; i<paramTypes.Length; ++i)
                {
                    if (i > 1)
                        sb.Append(',');
                    sb.Append(TypeToName(paramTypes[i]));
                }
                sb.Append('>');
                return sb.ToString();
            }
            return type.FullName;
        }
    }
}