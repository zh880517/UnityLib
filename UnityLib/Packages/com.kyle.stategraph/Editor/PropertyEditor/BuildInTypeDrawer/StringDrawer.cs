using UnityEditor;
using UnityEngine;
namespace PropertyEditor
{
    public class StringDrawer : TypeDrawer<string>
    {
        private string Value;
        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                if(content != null)
                    GUILayout.Label(content);
                Value = EditorGUILayout.TextField((string)val);
                return EditorGUI.EndChangeCheck();
            }
        }

        public override object GetValue()
        {
            return Value;
        }
    }

}
