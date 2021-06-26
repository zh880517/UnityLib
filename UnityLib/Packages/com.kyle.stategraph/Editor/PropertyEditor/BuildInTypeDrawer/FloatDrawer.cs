using UnityEditor;
namespace PropertyEditor
{
    public class FloatDrawer : ValueDrawer<float>
    {
        protected override void DoDraw(object val, StateGraph context)
        {
            Value = EditorGUILayout.FloatField(Value);
        }
    }

}