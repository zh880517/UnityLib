using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(GamePadKeyGroupConfig))]
public class GamePadKeyGroupConfigInspector : Editor
{
    private ReorderableList rangeList;

    private void OnEnable()
    {
        rangeList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Configs"),
                true, true, true, true);
        rangeList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "按键分组设置");
        };
        rangeList.elementHeightCallback = (int indexer) =>
        {
            return EditorGUIUtility.singleLineHeight + 1;
        };
        rangeList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = rangeList.serializedProperty.GetArrayElementAtIndex(index);
            Rect keyRect = new Rect(rect.x, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(keyRect, element.FindPropertyRelative("Key"), GUIContent.none);
            keyRect.x += keyRect.width;
            EditorGUI.PropertyField(keyRect, element.FindPropertyRelative("Group"), GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        rangeList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
