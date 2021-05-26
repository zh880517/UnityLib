using UnityEditor;
using UnityEngine;
namespace PropertyEditor
{
    public class FloatDrawer : ValueDrawer<float>
    {
        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            EditorGUI.BeginChangeCheck();
            Value = EditorGUILayout.FloatField(content, (float)val);
            return EditorGUI.EndChangeCheck();
        }
    }

}