using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
[CustomEditor(typeof(GamePadInputConfig))]
public class GamePadInputConfigInspector : Editor
{
    private ReorderableList rangeList;

    private void OnEnable()
    {
        rangeList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("InputKeys"),
                true, true, true, true);
        rangeList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "按键设置");
        };
        rangeList.elementHeightCallback = (int indexer) =>
        {
            return EditorGUIUtility.singleLineHeight * 3 + 2;
        };
        rangeList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 40;
            
            var element = rangeList.serializedProperty.GetArrayElementAtIndex(index);
            Rect keyRect = new Rect(rect.x, rect.y, rect.width * 0.5f, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(keyRect, element.FindPropertyRelative("Key"), new GUIContent("Key"));
            keyRect.x += keyRect.width;
            EditorGUI.PropertyField(keyRect, element.FindPropertyRelative("BtnName"), new GUIContent("按键", "Input中的名字，不填则摇杆值改变就触发"));
            
            keyRect.x = rect.x;
            keyRect.y += (EditorGUIUtility.singleLineHeight + 1);
            EditorGUI.PropertyField(keyRect, element.FindPropertyRelative("XAxis"), new GUIContent("XAxis", "Input中的名字，非摇杆类型不填"));
            keyRect.x += keyRect.width;
            EditorGUI.PropertyField(keyRect, element.FindPropertyRelative("YAxis"), new GUIContent("YAxis", "Input中的名字，非摇杆类型不填"));
            
            keyRect.x = rect.x;
            keyRect.width = rect.width;
            keyRect.y += (EditorGUIUtility.singleLineHeight + 1);
            EditorGUI.PropertyField(keyRect, element.FindPropertyRelative("Commit"), new GUIContent("描述"));
            EditorGUIUtility.labelWidth = oldWidth;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "InputKeys");
        GUILayout.Label("按钮有分组，同组的同一时间只能有一个响应\n同组按键优先级从上往下递减\n优先级高的已经按下则会跳过同组的后续按钮的检测\n键盘和手柄的输入是共享的，和UI的输入是互斥的");
        rangeList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
