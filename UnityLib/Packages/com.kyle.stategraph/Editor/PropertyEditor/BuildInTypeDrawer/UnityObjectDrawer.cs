using System;
using UnityEditor;
using UnityEngine;
namespace PropertyEditor
{
    public class UnityObjectDrawer : TypeDrawer<UnityEngine.Object>
    {
        private Type type;
        private UnityEngine.Object Value;
        public UnityObjectDrawer(Type type)
        {
            this.type = type;
        }

        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            Value = (UnityEngine.Object)val;
            using (new GUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                if (content != null)
                    GUILayout.Label(content);
                Value = EditorGUILayout.ObjectField(Value, type, false);
                return EditorGUI.EndChangeCheck();
            }
        }

        public override object GetValue()
        {
            return Value;
        }
    }
}