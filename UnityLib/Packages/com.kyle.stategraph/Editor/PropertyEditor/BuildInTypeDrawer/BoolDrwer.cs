using UnityEditor;

namespace PropertyEditor
{
    public class BoolDrwer : ValueDrawer<bool>
    {
        protected override void DoDraw(object val, StateGraph context)
        {
            Value = EditorGUILayout.Toggle((bool)val);
        }
    }

}