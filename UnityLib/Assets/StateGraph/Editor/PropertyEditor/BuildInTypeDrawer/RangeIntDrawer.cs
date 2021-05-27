using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public class RangeIntDrawer : CustomDrawer<RangeIntAttribute>
    {
        private int Value;
        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.Label(content);
            Value = EditorGUILayout.IntSlider((int)val, Attrbute.Min, Attrbute.Max);
            return EditorGUI.EndChangeCheck();
        }

        public override object GetValue()
        {
            return Value;
        }
    }

}

