using UnityEditor;
using UnityEngine;
namespace PropertyEditor
{
    public class StringDrawer : TypeDrawer<string>
    {
        private string Value;
        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            EditorGUI.BeginChangeCheck();
            Value = EditorGUILayout.TextField(content, (string)val);
            return EditorGUI.EndChangeCheck();
        }

        public override object GetValue()
        {
            return Value;
        }
    }

}
