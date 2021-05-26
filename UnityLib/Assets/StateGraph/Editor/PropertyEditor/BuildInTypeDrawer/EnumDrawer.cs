using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public class EnumDrawer : TypeDrawer<System.Enum>
    {
        private System.Enum Value;
        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            EditorGUI.BeginChangeCheck();
            Value = EditorGUILayout.EnumPopup(content, (System.Enum)val);
            return EditorGUI.EndChangeCheck();
        }

        public override object GetValue()
        {
            return Value;
        }
    }


}