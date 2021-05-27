using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public class RangeFloatDrawer : CustomDrawer<RangeFloatAttribute>
    {
        private float Value;
        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.Label(content);
            Value = EditorGUILayout.Slider((float)val, Attrbute.Min, Attrbute.Max);
            return EditorGUI.EndChangeCheck();
        }

        public override object GetValue()
        {
            return Value;
        }
    }

}
