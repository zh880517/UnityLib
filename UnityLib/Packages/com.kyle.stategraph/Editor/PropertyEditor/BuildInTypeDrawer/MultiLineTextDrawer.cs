using UnityEditor;
namespace PropertyEditor
{
    public class MultiLineTextDrawer : CustomDrawer<MultiLineTextAttribute>
    {
        private string Value;
        public override object GetValue()
        {
            return Value;
        }

        protected override void DoDraw(object val, StateGraph context)
        {
            Value = EditorGUILayout.TextArea((string)val);
        }
    }

}
