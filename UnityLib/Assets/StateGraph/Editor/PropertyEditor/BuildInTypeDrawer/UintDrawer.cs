using UnityEditor;

namespace PropertyEditor
{
    public class UintDrawer : ValueDrawer<uint>
    {
        public override void DoDraw(object val, StateGraph context)
        {
            Value = (uint)val;
            int newVal = EditorGUILayout.IntField((int)Value);
            if (newVal < 0)
            {
                Value = 0;
            }
        }
    }

}
