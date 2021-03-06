using System.Collections.Generic;
using UnityEditor;

public class StateSkillEditorWindow : StateGraphEditorWindow<StateSkillGraph, StateGraphView>
{
    [MenuItem("Tools/StateEditor/技能编辑器")]
    private static void OpenEditor()
    {
        GetWindow<StateSkillEditorWindow>();
    }

    [UnityEditor.Callbacks.OnOpenAsset(3)]
    public static bool OpenAsset(int instanceID, int line)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if (obj is StateSkillGraph skillGraph)
        {
            var window = GetWindow<StateSkillEditorWindow>("技能编辑器");
            window.Open(skillGraph);
            return true;
        }
        return false;
    }

    protected override IReadOnlyCollection<StateSkillGraph> GetGraphs()
    {
        return SkillGraphCollector.Instance.Graphs;
    }

    protected override void OpenCreateWizard()
    {
        ScriptableWizard.DisplayWizard<SkillCreateWizard>("创建技能");
    }
}
