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
                    var customDrawer = type.GetCustomAttribute<PropertyCustomDrawerAttribute>();
                    if (customDrawer != null)
                    {
                        drawerTypes[customDrawer.GetType()] = type;
                    }
                    else
                    {
                        var baseType = type.BaseType;
                        while(baseType != null)
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
        }

        public static IDrawer CreateDrawer(Type type)
        {
            if (Instance.drawerTypes.TryGetValue(type, out Type drawerType))
            {
                return Activator.CreateInstance(drawerType) as IDrawer;
            }
            if(type == null || type == typeof(object) || type.GetFields().Length == 0)
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
