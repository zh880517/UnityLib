using System;
using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public class EnumDrawer : TypeDrawer<Enum>
    {
        private EnumTypeInfo info;
        private int Value;
        public EnumDrawer(Type type)
        {
            info = EnumTypeInfo.Get(type);
        }
        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            Value = (int)val;
            EditorGUI.BeginChangeCheck();
            int idx = info.Values.IndexOf(Value);
            if (idx < 0)
            {
                idx = 0;
                GUI.changed = true;
            }
            if (content != null)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(content);
                    idx = EditorGUILayout.Popup(idx, info.Names);
                }
            }
            else
            {
                idx = EditorGUILayout.Popup(idx, info.Names);
            }
            Value = info.Values[idx];
            return EditorGUI.EndChangeCheck();
        }

        public override object GetValue()
        {
            return Value;
        }
    }


}