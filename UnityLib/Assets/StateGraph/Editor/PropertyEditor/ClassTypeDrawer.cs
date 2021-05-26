using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace PropertyEditor
{
    public class ClassTypeDrawer : IDrawer
    {
        public IDrawer BaseTypeDrawer;
        public List<FiledDrawer> Fields = new List<FiledDrawer>();
        private Type Type;

        public ClassTypeDrawer(Type type)
        {
            Type = type;
            BaseTypeDrawer = DrawerCollector.CreateDrawer(type.BaseType);
            foreach (var field in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public))
            {
                Fields.Add(new FiledDrawer(field));
            }
        }

        public bool Draw(GUIContent content, object val, StateGraph context)
        {
            if (content != null)
            {
                GUILayout.Label(content);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(10);
                    return Draw(val, context);
                }
            }
            else
            {
                return Draw(val, context);
            }
        }

        private bool Draw(object val, StateGraph context)
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
            using (new GUILayout.VerticalScope())
            {
                foreach (var field in Fields)
                {
                    field.Draw(val, context);
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