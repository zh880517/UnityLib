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

        public static IDrawer CreateDrawer(FieldInfo field)
        {
            if (field.IsStatic || !field.IsPublic || field.GetCustomAttribute<UnityEngine.HideInInspector>() != null)
                return null;
            var custom = field.GetCustomAttribute<PropertyCustomDrawerAttribute>();
            if (custom != null && custom.TypeCheck(field.FieldType))
            {
                if (Instance.drawerTypes.TryGetValue(custom.GetType(), out Type drawerType))
                {
                    var drawer = Activator.CreateInstance(drawerType) as CustomDrawerBase;
                    if (drawer != null)
                    {
                        drawer.SetAttribute(custom);
                        return drawer;
                    }
                }
            }
            if (field.FieldType.IsEnum && field.GetCustomAttribute<EnumMaskAttribute>() != null)
            {
                return new EnumMaskDrawer(field.FieldType);
            }
            return CreateDrawer(field.FieldType);
        }

        public static IDrawer CreateDrawer(Type type)
        {
            if (Instance.drawerTypes.TryGetValue(type, out Type drawerType))
            {
                var drawer = Activator.CreateInstance(drawerType) as IDrawer;
                return drawer;
            }
            if(type == null || type == typeof(object))
                return null;
            if (type.IsEnum)
            {
                return new EnumDrawer(type);
            }
            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                return new UnityObjectDrawer(type);
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
