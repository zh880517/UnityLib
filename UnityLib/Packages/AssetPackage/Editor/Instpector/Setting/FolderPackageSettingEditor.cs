using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AssetPackage
{
    [CustomEditor(typeof(FolderPackageSetting))]
    public class FolderPackageSettingEditor : Editor
    {
        private ReorderableList list;

        private void OnEnable()
        {
            list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Extensions"),
                true, true, false, true);

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element);
            };
            list.drawHeaderCallback = (Rect rect) =>
            {
                GUILayout.Label("资源名字通配符:", EditorStyles.boldLabel);
            };
        }
        public override void OnInspectorGUI()
        {
            FolderPackageSetting setting = target as FolderPackageSetting;
            GUILayout.Label("文件夹:", EditorStyles.boldLabel);
            using (new GUILayout.VerticalScope("Box"))
            {
                using (new GUILayout.VerticalScope())
                {
                    if (GUILayout.Button(setting.Folder, EditorStyles.linkLabel))
                    {
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(setting.Folder);
                    }
                    if (GUILayout.Button("选择"))
                    {
                        string defaultFolder = "Assets/";
                        if (!string.IsNullOrEmpty(setting.Folder))
                            defaultFolder = setting.Folder;
                        var folder = EditorUtility.OpenFolderPanel("选择文件夹", defaultFolder, string.Empty);
                        if (folder.StartsWith("Assets/"))
                        {
                            OnModify();
                            setting.Folder = folder;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("提示", string.Format("选择了无效的文件夹:\n{0}", folder), "确定");
                        }
                    }
                }
            }

            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            GUILayout.Label("资源列表:", EditorStyles.boldLabel);
            var files = setting.AssetPaths;
            HashSet<string> names = new HashSet<string>();
            using(new GUILayout.VerticalScope("Box"))
            {
                foreach (var file in files)
                {
                    using (new GUILayout.VerticalScope("Box"))
                    {
                        var obj = AssetDatabase.LoadMainAssetAtPath(file);
                        EditorGUILayout.ObjectField(obj, obj.GetType(), false);
                        if (names.Contains(obj.name))
                        {
                            GUILayout.Label("有重名的资源", "Wizard Error");
                        }
                        else if (GUILayout.Button(string.Format("{0}/{1}", setting.name, obj.name), "AC Button"))
                        {
                            names.Add(obj.name);
                            GUIUtility.systemCopyBuffer = string.Format("{0}/{1}", setting.name, obj.name);
                        }

                    }
                }

            }
        }

        private void OnModify()
        {
            Undo.RegisterCompleteObjectUndo(target, "modify folder package setting");
            EditorUtility.SetDirty(target);
        }
    }
}