using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace PropertyEditor
{
    public class DrawerCollector
    {
        static DrawerCollector _instance;
        private Dictionary<Type, Type> drawerTypes = new Dictionary<Type, Type>();
        public static DrawerCollector Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DrawerCollector();
                }
                return _instance;
            }
        }

        private DrawerCollector()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.FullName.StartsWith("Unity") 
                    || assembly.FullName.StartsWith("com.unity")
                    || assembly.FullName.StartsWith("System")
                    || assembly.FullName.StartsWith("mscorlib") )
                {
                    continue;
                }
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsAbstract || type.IsInterface || type.IsGenericType)
                        continue;
                    if (!typeof(IDrawer).IsAssignableFrom(type))
                        continue;
                    var baseType = type.BaseType;
                    while (baseType != null)
                    {
                        if (baseType.IsGenericType)
                        {
                            drawerTypes[baseType.GenericTypeArguments[0]] = type;
                            break;
                        }
                        baseType = baseType.BaseType;
                    }
                }
            }
        }

        public static IDrawer CreateDrawer(Type type, PropertyCustomDrawerAttribute attribute = null)
        {
            if (Instance.drawerTypes.TryGetValue(type, out Type drawerType))
            {
                var drawer = Activator.CreateInstance(drawerType) as IDrawer;
                if (attribute != null && drawer is CustomDrawerBase customDrawer)
                {
                    customDrawer.SetAttribute(attribute);
                }
                return drawer;
            }
            if(type == null || type == typeof(object))
                return null;
            if (type.IsEnum)
            {
                if (type.GetCustomAttribute<EnumMaskAttribute>() != null)
                {
                    return new EnumMaskDrawer();
                }
                return new EnumDrawer();
            }
            if (type.IsValueType)
            {
                if (type == typeof(short)  || type == typeof(long))
                {
                    return new IntDrawer();
                }
                else if (type == typeof(ushort) || type == typeof(ulong))
                {
                    return new UintDrawer();
                }
                else if (type == typeof(double))
                {
                    return new FloatDrawer();
                }

                return new StructTypeDrawer(type);
            }
            else if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    return new ListDrawer(type);
                }
                //只支持List容器
                if (typeof(IEnumerable).IsAssignableFrom(type))
                    return null;
            }
            return new ClassTypeDrawer(type);
        }

        public static void OnPropertyModify(StateGraph graph)
        {
            UnityEditor.Undo.RegisterCompleteObjectUndo(graph, "Property Modify");
            UnityEditor.EditorUtility.SetDirty(graph);
        }
    }

}
