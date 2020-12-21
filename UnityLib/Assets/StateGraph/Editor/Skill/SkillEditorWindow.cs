using UnityEditor;

public class SkillEditorWindow : StateGraphEditorWindow
{
    [UnityEditor.Callbacks.OnOpenAsset(3)]
    public static bool OpenAsset(int instanceID, int line)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if (obj is StateSkillGraph skillGraph)
        {
            var window = GetWindow<SkillEditorWindow>("技能编辑器");
            if (window.View == null || window.View.Graph != skillGraph)
            {
                window.View = CreateInstance<StateGraphView>();
                window.View.Init(skillGraph);
                Undo.RecordObject(window, string.Format("open skill {0}", skillGraph.name));
            }
            return true;
        }
        return false;
    }


}
