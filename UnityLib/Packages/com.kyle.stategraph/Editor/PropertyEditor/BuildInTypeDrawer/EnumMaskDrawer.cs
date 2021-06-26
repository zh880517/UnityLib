using System;
using UnityEditor;
using UnityEngine;
namespace PropertyEditor
{
    public class EnumMaskDrawer : TypeDrawer<Enum>
    {
        private int Value;
        private EnumTypeInfo info;
        public EnumMaskDrawer(Type type)
        {
            info = EnumTypeInfo.Get(type, true);
        }
        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            Value = (int)val;
            using (new GUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                GUILayout.Label(content);
                Value = EditorGUILayout.MaskField(Value, info.Names);
                return EditorGUI.EndChangeCheck();
            }
        }

        public override object GetValue()
        {
            return Value;
        }
    }

}