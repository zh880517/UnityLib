using UnityEditor;
using UnityEngine;
namespace PropertyEditor
{
    public class IntDrawer : ValueDrawer<int>
    {
        public override void DoDraw(object val, StateGraph context)
        {
            Value = EditorGUILayout.IntField((int)val);
        }
    }

}