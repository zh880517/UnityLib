using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public class StructTypeDrawer : IDrawer
    {
        public IDrawer BaseTypeDrawer;
        public List<FiledDrawer> Fields = new List<FiledDrawer>();
        private object value;
        private bool foldout = true;

        public StructTypeDrawer(Type type)
        {
            BaseTypeDrawer = DrawerCollector.CreateDrawer(type.BaseType);
            foreach (var field in type.GetFields())
            {
                if (field.DeclaringType != type)
                    continue;
                var fieldDrawer = FiledDrawer.Create(field);
                if (fieldDrawer != null)
                    Fields.Add(fieldDrawer);
            }
        }

        public bool Draw(GUIContent content, object val, StateGraph context)
        {
            if (content != null)
            {
                foldout = EditorGUILayout.Foldout(foldout, content);
                if (!foldout)
                    return false;
            }
            return Draw(val, context, content != null);
        }

        private bool Draw(object val, StateGraph context, bool offset)
        {
            bool modify = false;
            if (BaseTypeDrawer != null)
            {
                modify = modify || BaseTypeDrawer.Draw(null, val, context);
            }
            using (new GUILayout.VerticalScope(offset ? ClassTypeDrawer.ContentStyle : ""))
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