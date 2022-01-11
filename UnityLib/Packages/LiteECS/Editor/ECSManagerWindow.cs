using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LiteECS.Editor
{
    public class ECSManagerWindow : EditorWindow
    {
        [MenuItem("ECS/管理器")]
        public static void Open()
        {
            GetWindow<ECSManagerWindow>("ECS管理器");
        }
        private string createName;
        private string createPath;
        private bool isCreateing;

        private void DrawCreate()
        {
            if (isCreateing)
            {
                createName = GUILayout.TextField(createName);
                if (createName != null && createName.EndsWith("Context", StringComparison.OrdinalIgnoreCase))
                {
                    Color color = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;
                    GUILayout.Label("名字不需要包含 Context");
                    GUI.backgroundColor = color;
                }
                if (GUILayout.Button(string.IsNullOrWhiteSpace(createPath) ? "选择目录" : createPath , EditorStyles.linkLabel))
                {
                    var select = EditorUtility.OpenFolderPanel("选择目录", "Assets/", "");
                    if (!string.IsNullOrEmpty(select))
                    {
                        var projectPath = Environment.CurrentDirectory.Replace("\\", "/");
                        if (!projectPath.EndsWith("/"))
                            projectPath += "/";

                        select = Path.GetFullPath(select).Replace("\\", "/").Replace(projectPath, "");
                        if (!select.StartsWith("Assets/"))
                        {
                            EditorUtility.DisplayDialog("提示", "请选择Unity工程内的文件", "确定");
                            return;
                        }
                        createPath = select;
                    }
                }
                using(new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("放弃"))
                    {
                        isCreateing = false;
                    }
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("创建"))
                    {
                        var config = ECSConfig.Instance.AddContext(createName, createPath);
                        if ( config != null)
                        {
                            config.InitECSDirectory();
                            AssetDatabase.Refresh();
                            isCreateing = false;
                            createName = "";
                            createPath = "";
                        }
                    }
                }
            }
            if (!isCreateing && GUILayout.Button("创建"))
            {
                isCreateing = true;
            }
        }

        private void DrawAddExits()
        {
            if (GUILayout.Button("从目录添加"))
            {
                var select = EditorUtility.OpenFilePanel("选择已经存在ECS文件", "Assets/", "cs");
                if (!string.IsNullOrEmpty(select))
                {
                    var projectPath = Environment.CurrentDirectory.Replace("\\", "/");
                    if (!projectPath.EndsWith("/"))
                        projectPath += "/";

                    select = Path.GetFullPath(select).Replace("\\", "/").Replace(projectPath, "");
                    if (!select.StartsWith("Assets/"))
                    {
                        EditorUtility.DisplayDialog("提示", "请选择Unity工程内的文件", "确定");
                        return;
                    }
                    string fileName = Path.GetFileName(select);
                    if (!fileName.EndsWith("ECS.cs"))
                    {
                        EditorUtility.DisplayDialog("提示", "无效的文件，ECS文件是以{{ContentName}}ECS.cs命名的", "确定");
                        return;
                    }
                    string name = fileName.Substring(0, fileName.Length - 6);
                    string path = select.Substring(0, select.Length - fileName.Length);
                    var config = ECSConfig.Instance.AddContext(name, path);
                    if (config != null)
                    {
                        config.InitECSDirectory();
                        AssetDatabase.Refresh();
                    }
                }
            }
        }

        void OnGUI()
        {
            for(int i=0; i< ECSConfig.Instance.Contexts.Count; ++i)
            {
                var context = ECSConfig.Instance.Contexts[i];
                using (new EditorGUILayout.VerticalScope("Box"))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Label(context.Name);
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("移除"))
                        {
                            ECSConfig.MakeModify("remove context");
                            ECSConfig.Instance.Contexts.RemoveAt(i);
                            --i;
                            continue;
                        }
                    }
                    GUILayout.Space(5);
                    if (GUILayout.Button(context.DirectoryPath, EditorStyles.linkLabel))
                    {
                        string selectPath = context.DirectoryPath;
                        if (selectPath.EndsWith("/"))
                            selectPath = selectPath.Substring(0, selectPath.Length - 1);
                        var obj = AssetDatabase.LoadMainAssetAtPath(selectPath);
                        EditorGUIUtility.PingObject(obj);
                        Selection.activeObject = obj;
                    }
                }
                GUILayout.Space(10);
            }
            DrawCreate();
            DrawAddExits();
            if (ECSConfig.IsModfy() && GUILayout.Button("保存"))
            {
                ECSConfig.SaveToFile();
            }
        }

        private void OnDestroy()
        {
            if (ECSConfig.IsModfy())
            {
                if (EditorUtility.DisplayDialog("提示", "ECS有未保存的修改，是否保存？", "保存", "放弃"))
                {
                    ECSConfig.SaveToFile();
                }
                else
                {
                    ECSConfig.ReloadFormFile();
                }
            }
            //Undo.ClearUndo(ECSConfig.Instance);
        }
    }

}