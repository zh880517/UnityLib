using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeGenerator
{
    public class TypeCollector<TBase>
    {
        private static List<Type> _types;

        public static List<Type> Types
        {
            get
            {
                if (_types == null)
                {
                    foreach (var assemble in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        CollectType(assemble, _types);
                    }
                }
                return _types;
            }
        }

        public static void CollectType(Assembly assembly, List<Type> types, Func<Type, bool> condition = null)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;
                if (typeof(TBase).IsAssignableFrom(type) && (condition == null || condition(type)))
                {
                    types.Add(type);
                }
            }
        }
    }

}

