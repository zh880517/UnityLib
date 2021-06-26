using UnityEditor;

namespace PropertyEditor
{
    public class RangeIntDrawer : CustomDrawer<RangeIntAttribute>
    {
        private int Value;
        protected override void DoDraw(object val, StateGraph context)
        {
            Value = EditorGUILayout.IntSlider((int)val, Attrbute.Min, Attrbute.Max);
        }

        public override object GetValue()
        {
            return Value;
        }
    }

}

