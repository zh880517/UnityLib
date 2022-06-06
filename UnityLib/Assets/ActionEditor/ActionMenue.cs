using UnityEditor;
using UnityEngine;

public static class ActionMenue
{
    internal class DoCreateAction : UnityEditor.ProjectWindowCallback.EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            ActionAsset asset = CreateInstance<ActionAsset>();
            asset.FrameCount = 100;
            AssetDatabase.CreateAsset(asset, pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
    [MenuItem("Assets/Create/动作编辑")]
    static void CreateAsset()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateAction>(), "New Action.asset", null, null);
    }

    [UnityEditor.Callbacks.OnOpenAsset(3)]
    static bool OpenAsset(int instanceID, int line)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if (obj is ActionAsset actionAsset)
        {
            ActionEditorWinow.Open(actionAsset);
        }
        return false;
    }
}
