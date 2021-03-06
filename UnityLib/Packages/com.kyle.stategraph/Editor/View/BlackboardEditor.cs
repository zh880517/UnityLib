using UnityEditor;
using UnityEngine;
[System.Serializable]
public class BlackboardEditor
{
    public string InputName;
    public Vector2 ScrollPos;
    public void Draw(StateGraphView view)
    {
        ScrollPos = GUILayout.BeginScrollView(ScrollPos);
        using(new GUILayout.HorizontalScope())
        {
            GUILayout.Label("Name");
            InputName = GUILayout.TextField(InputName);
            if (!string.IsNullOrWhiteSpace(InputName) && GUILayout.Button("创建"))
            {
                if (view.Graph.Blackboard.HasName(InputName))
                {
                    EditorUtility.DisplayDialog("错误", string.Format("创建失败：已经存在 {0} 变量", InputName), "确定");
                }
                else
                {
                    view.RegistUndo("add variable");
                    view.Graph.Blackboard.Add(InputName);
                }
            }
        }
        
        for (int i=0; i<view.Graph.Blackboard.Variables.Count; ++i)
        {
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("", "SearchCancelButton"))
                {
                    view.RegistUndo("remove variable");
                    view.Graph.Blackboard.RemoveAt(i);
                    i--;
                    continue;
                }
                var variable = view.Graph.Blackboard.Variables[i];
                GUILayout.Label(variable.Name);
                float val = EditorGUILayout.FloatField(variable.DefultValue);
                if (val != variable.DefultValue)
                {
                    view.RegistUndo("modify variable defult value");
                    variable.DefultValue = val;
                }
            }
        }
        GUILayout.EndScrollView();
    }
}
