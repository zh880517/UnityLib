using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace AssetPackage
{
    [CustomEditor(typeof(AssetPackageBuildSetting))]
    public class AssetPackageBuildSettingEditor : Editor
    {
        private ReorderableList list;
        static string inputName;

        private void OnEnable()
        {
            list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Packages"),
                true, true, false, true);

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(rect, element.objectReferenceValue, element.objectReferenceValue.GetType(), false);
                //EditorGUI.PropertyField(rect, element);
            };
        }

        public override void OnInspectorGUI()
        {
            var buildSetting = target as AssetPackageBuildSetting;

            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "Packages");
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            using(new EditorGUILayout.VerticalScope("Box"))
            {
                GUILayout.Label("输入将要创建的资源包名：");
                inputName = GUILayout.TextField(inputName);
                if (GUILayout.Button("添加文件夹打包"))
                {
                    AddPackage<FolderPackageSetting>(inputName);
                }
                if (GUILayout.Button("添加零散文件打包"))
                {
                    AddPackage<FilesPackageSetting>(inputName);
                }
            }
            if (GUILayout.Button("选择并添加已存在的打包配置"))
            {
                var guids = AssetDatabase.FindAssets("t:AssetPackageSetting", new string[] { AssetPackageBuildSetting.SettingFolder });
                GenericMenu genericMenu = new GenericMenu();
                foreach (var guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    var setting = AssetDatabase.LoadAssetAtPath<AssetPackageSetting>(path);
                    if (!buildSetting.Packages.Contains(setting))
                    {
                        genericMenu.AddItem(new GUIContent(setting.name), false, () => { AddPackage(setting); });
                    }
                }
                genericMenu.ShowAsContext();
            }
        }

        private T AddPackage<T>(string name) where T : AssetPackageSetting
        {
            var buildSetting = target as AssetPackageBuildSetting;
            string savePath = string.Format("{0}{1}.asset", AssetPackageBuildSetting.SettingFolder, name);
            if (File.Exists(savePath) || buildSetting.Packages.Exists(it=>it.name == name))
            {
                EditorUtility.DisplayDialog("错误", string.Format("名字不可用，已经有重复的 ：{0}", name), "确定");
                return null;
            }
            T setting = CreateInstance<T>();
            setting.name = name;
            AssetDatabase.CreateAsset(setting, savePath);
            AddPackage(setting);
            return setting;
        }

        private void AddPackage(AssetPackageSetting setting)
        {
            var buildSetting = target as AssetPackageBuildSetting;
            Undo.RegisterCompleteObjectUndo(target, "add package setting");
            buildSetting.Packages.Add(setting);
            EditorUtility.SetDirty(target);
        }
    }
}
