using UnityEditor;

namespace PropertyEditor
{
    public class UintDrawer : ValueDrawer<uint>
    {
        protected override void DoDraw(object val, StateGraph context)
        {
            Value = (uint)val;
            int newValue = EditorGUILayout.IntField((int)Value);

            if (newValue < 0)
                Value = 0;
            else
                Value = (uint)newValue;
        }
    }

}
