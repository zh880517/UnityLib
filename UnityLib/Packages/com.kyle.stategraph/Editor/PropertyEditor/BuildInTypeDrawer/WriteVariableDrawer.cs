using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public class WriteVariableDrawer : ValueDrawer<WriteVariable>
    {
        protected override void DoDraw(object val, StateGraph context)
        {
            int idx = context.Blackboard.NameIndex(Value.Key);
            if (idx < 0)
            {
                Color color = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button(Value.Key))
                {
                    if (context.Blackboard.Names.Length == 0)
                    {
                        Value.Key = string.Empty;
                    }
                    else
                    {
                        Value.Key = context.Blackboard.Names[0];
                    }
                    GUI.changed = true;
                }
                GUI.backgroundColor = color;
            }
            else
            {
                idx = EditorGUILayout.Popup(idx, context.Blackboard.Names);
                Value.Key = context.Blackboard.Names[idx];
            }
        }
    }
}