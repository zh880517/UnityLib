using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace PropertyEditor
{
    public class ClassTypeDrawer : IDrawer
    {
        public IDrawer BaseTypeDrawer;
        public List<FieldDrawer> Fields = new List<FieldDrawer>();
        private Type Type;
        private bool foldout = true;
        private readonly MethodInfo ValidMethod;

        public static GUIStyle ContentStyle = new GUIStyle() { padding = new RectOffset(10, 0, 0, 0) };
        public static GUIStyle Empty = new GUIStyle();
        public ClassTypeDrawer(Type type)
        {
            Type = type;
            var validFunc = type.GetCustomAttribute<ValidFuncAttribute>();
            if (validFunc != null)
            {
                ValidMethod = type.GetMethod(validFunc.Name);
            }
            BaseTypeDrawer = DrawerCollector.CreateDrawer(type.BaseType);
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.DeclaringType != type)
                    continue;
                var fieldDrawer = FieldDrawer.Create(field);
                if (fieldDrawer != null)
                    Fields.Add(fieldDrawer);
            }
        }

        public bool Draw(GUIContent content, object val, StateGraph context)
        {
            if (content != null)
            {
                foldout = EditorGUILayout.Foldout(foldout, content, true);
                if (!foldout)
                    return false;
            }
            return Draw(val, context, content != null);
        }

        private bool Draw(object val, StateGraph context, bool offset)
        {
            if (val == null)
            {
                DrawerCollector.OnPropertyModify(context);
                return true;
            }
            if (BaseTypeDrawer != null)
            {
                BaseTypeDrawer.Draw(null, val, context);
            }
            if (ValidMethod == null || (bool)ValidMethod.Invoke(val, null))
            {
                using (new GUILayout.VerticalScope(offset ? ContentStyle : Empty))
                {
                    foreach (var field in Fields)
                    {
                        field.Draw(val, context);
                    }
                }
            }
            return false;
        }

        public object GetValue()
        {
            //引用类型只有在初始值为null的是才会调用这个接口
            return Activator.CreateInstance(Type);
        }
    }
}