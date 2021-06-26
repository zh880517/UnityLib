using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public class ReadVariableDrawer : ValueDrawer<ReadVariable>
    {
        protected override void DoDraw(object val, StateGraph context)
        {
            if (!Value.Share)
            {
                Value.Value = EditorGUILayout.FloatField(Value.Value);
            }
            else
            {
                int idx = context.Blackboard.NameIndex(Value.Key);
                if (idx < 0)
                {
                    Color color = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button(Value.Key, GUILayout.MinWidth(50)))
                    {
                        if (context.Blackboard.Names.Length == 0)
                        {
                            Value.Share = false;
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
            Value.Share = EditorGUILayout.Toggle(Value.Share, GUILayout.Width(15));
            if (Value.Share && string.IsNullOrEmpty(Value.Key) && context.Blackboard.Names.Length > 0)
            {
                Value.Key = context.Blackboard.Names[0];
            }
        }
    }

}
