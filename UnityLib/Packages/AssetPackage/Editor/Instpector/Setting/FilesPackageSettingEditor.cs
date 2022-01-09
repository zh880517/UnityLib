using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AssetPackage
{
    [CustomEditor(typeof(FilesPackageSetting))]
    public class FilesPackageSettingEditor : Editor
    {
        private ReorderableList list;
        private void OnEnable()
        {
            list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Assets"),
                true, true, true, true);
            list.elementHeightCallback = (int index) =>
            {
                return EditorGUIUtility.singleLineHeight * 2 + 1;
            };
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                Rect topRect = rect;
                topRect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(topRect, element);
                var obj = element.objectReferenceValue;
                topRect.y += EditorGUIUtility.singleLineHeight;
                if (AssetDatabase.IsMainAsset(obj))
                {
                    EditorGUI.LabelField(topRect, "该资源无法被打包", "Wizard Error");
                }
                else if(GUI.Button(topRect, string.Format("{0}/{1}", target.name, obj.name), "AC Button"))
                {
                    GUIUtility.systemCopyBuffer = string.Format("{0}/{1}", target.name, obj.name);
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "Packages");
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}