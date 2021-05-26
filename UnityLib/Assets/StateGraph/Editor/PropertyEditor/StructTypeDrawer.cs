using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PropertyEditor
{
    public class StructTypeDrawer : IDrawer
    {
        public IDrawer BaseTypeDrawer;
        public List<FiledDrawer> Fields = new List<FiledDrawer>();
        private object value;

        public StructTypeDrawer(Type type)
        {
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
            bool modify = false;
            if (BaseTypeDrawer != null)
            {
                modify = modify || BaseTypeDrawer.Draw(null, val, context);
            }
            using (new GUILayout.VerticalScope())
            {
                foreach (var field in Fields)
                {
                    modify = modify || field.Draw(val, context);
                }
            }
            value = val;
            return modify;
        }

        public object GetValue()
        {
            return value;
        }
    }


}