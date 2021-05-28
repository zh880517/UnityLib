using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public class ReadVariableDrawer : ValueDrawer<ReadVariable>
    {
        public override void DoDraw(object val, StateGraph context)
        {
            Value = (ReadVariable)val;
            if (!Value.UseBlackBoard)
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
                            Value.UseBlackBoard = false;
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
            Value.UseBlackBoard = EditorGUILayout.Toggle(Value.UseBlackBoard, GUILayout.Width(15));
            if (Value.UseBlackBoard && string.IsNullOrEmpty(Value.Key) && context.Blackboard.Names.Length > 0)
            {
                Value.Key = context.Blackboard.Names[0];
            }
        }
    }

}
