using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public class EnumDrawer : TypeDrawer<System.Enum>
    {
        private System.Enum Value;
        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            using(new GUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                GUILayout.Label(content);
                Value = EditorGUILayout.EnumPopup((System.Enum)val);
                return EditorGUI.EndChangeCheck();
            }
        }

        public override object GetValue()
        {
            return Value;
        }
    }


}