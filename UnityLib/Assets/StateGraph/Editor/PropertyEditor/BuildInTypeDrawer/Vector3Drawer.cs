using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public class Vector3Drawer : ValueDrawer<Vector3>
    {
        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            EditorGUI.BeginChangeCheck();
            Value = EditorGUILayout.Vector3Field(content, (Vector3)val);
            return EditorGUI.EndChangeCheck();
        }
    }

}
