using UnityEditor;
using UnityEngine;
namespace PropertyEditor
{
    public class IntDrawer : ValueDrawer<int>
    {
        protected override void DoDraw(object val, StateGraph context)
        {
            Value = EditorGUILayout.IntField(Value);
        }
    }

}