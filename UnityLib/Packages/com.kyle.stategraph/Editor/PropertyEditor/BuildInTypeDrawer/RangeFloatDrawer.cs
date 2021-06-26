using UnityEditor;

namespace PropertyEditor
{
    public class RangeFloatDrawer : CustomDrawer<RangeFloatAttribute>
    {
        private float Value;
        protected override void DoDraw(object val, StateGraph context)
        {
            Value = EditorGUILayout.Slider((float)val, Attrbute.Min, Attrbute.Max);
        }

        public override object GetValue()
        {
            return Value;
        }
    }

}
