using UnityEditor;
using UnityEngine;
namespace PropertyEditor
{
    public class IntDrawer : ValueDrawer<int>
    {
        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            EditorGUI.BeginChangeCheck();
            Value = EditorGUILayout.IntField(content, (int)val);
            return EditorGUI.EndChangeCheck();
        }
    }

}