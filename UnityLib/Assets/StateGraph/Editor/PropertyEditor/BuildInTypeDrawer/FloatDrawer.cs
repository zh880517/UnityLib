using UnityEditor;
namespace PropertyEditor
{
    public class FloatDrawer : ValueDrawer<float>
    {
        public override void DoDraw(object val, StateGraph context)
        {
            Value = EditorGUILayout.FloatField((float)val);
        }
    }

}