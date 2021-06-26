using System;
using System.Collections.Generic;
using System.Reflection;

namespace PropertyEditor
{
    public class EnumTypeInfo
    {
        private readonly static Dictionary<Type, EnumTypeInfo> NormalCache = new Dictionary<Type, EnumTypeInfo>();
        private readonly static Dictionary<Type, EnumTypeInfo> MaskCache = new Dictionary<Type, EnumTypeInfo>();

        public static EnumTypeInfo Get(Type type, bool mask = false)
        {
            if (!mask)
            {
                if (!NormalCache.TryGetValue(type, out var info))
                {
                    info = new EnumTypeInfo(type, false);
                    NormalCache.Add(type, info);

                }
                return info;
            }
            else
            {
                if (!MaskCache.TryGetValue(type, out var info))
                {
                    info = new EnumTypeInfo(type, true);
                    MaskCache.Add(type, info);

                }
                return info;
            }
        }

        public List<int> Values { get; private set; } = new List<int>();

        public string[] Names { get; private set; }
        public Type EnumType { get; private set; }

        EnumTypeInfo(Type type, bool mask)
        {
            EnumType = type;
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            var values = Enum.GetValues(type);
            foreach (var val in values)
            {
                Values.Add((int)val);
            }
            var names = new List<string>();
            for (int i=0; i<fields.Length; ++i)
            {
                var field = fields[i];
                var disPlayName = field.GetCustomAttribute<DisplayNameAttribute>();
                if (disPlayName != null)
                    names.Add(disPlayName.Name);
                else
                    names.Add(field.Name);
            }
            if (mask)
            {
                for (int i= Values.Count - 1; i>=0; --i)
                {
                    if (Values[i] == 0 || Values[i] == -1)
                    {
                        Values.RemoveAt(i);
                        names.RemoveAt(i);
                    }
                }
            }
            Names = names.ToArray();
        }
    }
}