using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public class Vector3Drawer : ValueDrawer<Vector3>
    {
        public override void DoDraw(object val, StateGraph context)
        {
            Value = EditorGUILayout.Vector3Field("",(Vector3)val);
        }
    }

}
