using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StateGraphGroup))]
public class StateGraphGroupInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("组仅为显示用，只能增加和修改名字，不能删除");
        StateGraphGroup owner = target as StateGraphGroup;
        for (int i=0; i<owner.Groups.Count; ++i)
        {
            string name = owner.Groups[i];
            EditorGUI.BeginChangeCheck();
            name = EditorGUILayout.TextField(i.ToString(), name);
            if (EditorGUI.EndChangeCheck())
            {
                OnModify();
                owner.Groups[i] = name;
            }
        }
        if (GUILayout.Button("添加"))
        {
            OnModify();
            int newId = owner.Groups.Count;
            owner.Groups.Add($"Group {newId}");
        }
    }

    private void OnModify()
    {
        Undo.RegisterCompleteObjectUndo(target, "modify ability attribute");
        EditorUtility.SetDirty(target);
    }
}
