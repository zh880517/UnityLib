using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace LiteECS.Editor
{
    public class ECSCreateWizard : ScriptableWizard
    {
        public enum CreateType
        {
            Component,
            System,
        }
        public bool OpenAfterCreate = false;
        protected string[] contextNames;
        private string[][] catalogs;
        private int selectContextIdx;
        private int[] selectCatalogIdx = new int[2];
        private CreateType createType;
        private ECSSystemGenerateType systemType;
        private string createCatalogName = "";
        private string nameInput = "";
        private string[] componentTypes;
        private int componentSelectIdx;
        protected override bool DrawWizardGUI()
        {
            base.DrawWizardGUI();
            if (contextNames == null)
            {
                return false;
            }
            if (contextNames.Length == 0)
            {
                errorString = "当前没有有效的Context，请添加Context后再创建";
                return false;
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Context");
                EditorGUI.BeginChangeCheck();
                selectContextIdx = EditorGUILayout.Popup(selectContextIdx, contextNames);
                if (EditorGUI.EndChangeCheck())
                {
                    UpdateContextSelect();
                }
            }
            if (catalogs == null)
                return false;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("创建类型");
                createType = (CreateType)EditorGUILayout.EnumPopup(createType);
            }
            int typeIdx = (int)createType;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("选择目录");
                selectCatalogIdx[typeIdx] = EditorGUILayout.Popup(selectCatalogIdx[typeIdx], catalogs[typeIdx]);
            }
            DrawCreateCatalog();
            if (createType == CreateType.Component)
            {
                DrawCreateComponent();
            }
            else if (createType == CreateType.System)
            {
                DrawCreateSystem();
            }
            return false;
        }

        private void DrawCreateCatalog()
        {
            using (new EditorGUILayout.VerticalScope("Box"))
            {
                int typeIdx = (int)createType;
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("新建子目录名字");
                    createCatalogName = GUILayout.TextField(createCatalogName);
                }
                if (GUILayout.Button("在选择的目录创建子目录"))
                {
                    int idx = createCatalogName.IndexOfAny(Path.GetInvalidFileNameChars());
                    if (idx >= 0)
                    {
                        errorString = $"目录名字里面含有非法字符 {createCatalogName[idx]}";
                    }
                    else
                    {
                        string catalog = catalogs[typeIdx][selectCatalogIdx[typeIdx]];
                        string newCatalog = $"{catalog}/{createCatalogName}";
                        if (catalogs[typeIdx].Contains(newCatalog))
                        {
                            errorString = $"创建失败，创建的目录已存在 {newCatalog}";
                            return;
                        }
                        var cfg = ECSConfig.Instance.Contexts[selectContextIdx];
                        string path = Path.Combine(cfg.DirectoryPath, newCatalog);
                        try
                        {
                            Directory.CreateDirectory(path);
                            List<string> result = null;
                            if (createType == CreateType.Component)
                            {
                                result = cfg.GetComponentCatalog();
                            }
                            else if (createType == CreateType.System)
                            {
                                result = cfg.GetSystemsCatalog();
                            }
                            catalogs[typeIdx] = result.ToArray();
                            selectCatalogIdx[typeIdx] = result.IndexOf(newCatalog);
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogException(ex);
                            return;
                        }
                    }
                }

            }
        }

        private void DrawCreateComponent()
        {
            GUILayout.Label("输入需要创建的Component的名字 : ", EditorStyles.boldLabel);
            DrawNameInput();
            GUILayout.Label("为了防止多个Context有重名的组件以及减少文件名长度，名字不需要带Component，创建时会自动加上Context的名字做前缀", EditorStyles.wordWrappedLabel);
        }


        private void DrawCreateSystem()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("系统类型");
                systemType = (ECSSystemGenerateType)EditorGUILayout.EnumPopup(systemType);
            }
            if (systemType >= ECSSystemGenerateType.GroupExecute && componentTypes != null)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("指定组件类型");
                    componentSelectIdx = EditorGUILayout.Popup(componentSelectIdx, componentTypes);
                }
            }
            GUILayout.Label("输入需要创建的System的名字 : ", EditorStyles.boldLabel);
            DrawNameInput();
            GUILayout.Label("为了防止多个Context有重名的System，创建时会自动加上Context的名字做前缀，加上System做后缀和组件类名做区分", EditorStyles.wordWrappedLabel);
        }

        private void DrawNameInput()
        {
            EditorGUI.BeginChangeCheck();
            nameInput = GUILayout.TextField(nameInput);
            if (EditorGUI.EndChangeCheck())
            {
                int idx = nameInput.IndexOfAny(Path.GetInvalidFileNameChars());
                if (idx >= 0)
                {
                    errorString = $"名字里面含有非法字符 {nameInput[idx]}";
                }
            }
        }

        private bool UpdateContextSelect()
        {
            if (contextNames == null)
            {
                contextNames = ECSConfig.Instance.Contexts.Select(it => it.Name).ToArray();
            }
            catalogs = null;
            componentTypes = null;
            string name = contextNames[selectContextIdx];
            var cfg = ECSConfig.Instance.Contexts[selectContextIdx];
            if (cfg == null)
            {
                contextNames = ECSConfig.Instance.Contexts.Select(it => it.Name).ToArray();
                selectContextIdx = 0;
                if (contextNames.Length == 0)
                    return false;
                cfg = ECSConfig.Instance.Contexts[selectContextIdx];
            }
            catalogs = new string[][]
            {
            cfg.GetComponentCatalog().ToArray(),
            cfg.GetSystemsCatalog().ToArray()
            };
            selectCatalogIdx = new int[2];
            var types = cfg.ComponentTypes;
            if (types != null)
            {
                componentTypes = types.Select(it => it.FullName).ToArray();
            }
            componentSelectIdx = 0;
            return true;
        }

        private void OnWizardCreate()
        {
            var cfg = ECSConfig.Instance.Contexts.Find(it => it.Name == contextNames[selectContextIdx]);
            if (cfg == null)
            {
                EditorUtility.DisplayDialog("错误", "选择的Context不存在，可能被移动位置或者删除\n,请重新创建", "ok");
                return;
            }
            int typeIdx = (int)createType;
            string dirPath = Path.Combine(cfg.DirectoryPath, catalogs[typeIdx][selectCatalogIdx[typeIdx]]);
            string className = nameInput;
            if (createType == CreateType.Component)
            {
                if (!nameInput.StartsWith(cfg.Name))
                    className = $"{cfg.Name}{nameInput}";
            }
            else if (createType == CreateType.System)
            {
                if (!nameInput.StartsWith(cfg.Name))
                    className = $"{cfg.Name}{nameInput}System";
                else
                    className = $"{nameInput}System";
            }
            System.Reflection.Assembly assembly = ECSContextUtils.GetECSAssembly(cfg);
            if (assembly != null)
            {
                if (assembly.GetType(className) != null)
                {
                    EditorUtility.DisplayDialog("错误", $"创建失败，类型 {className} 已存在", "ok");
                    return;
                }
            }
            string filePath = Path.Combine(dirPath, $"{className}.cs");
            if (File.Exists(filePath))
            {
                EditorUtility.DisplayDialog("错误", "创建失败，目标文件已存在", "ok");
                return;
            }
            string content = null;
            if (createType == CreateType.Component)
            {
                CodeWriter writer = new CodeWriter(true);
                writer.Write($"public partial class {className} : I{cfg.Name}Component");
                writer.EmptyScop();
                content = writer.ToString();
            }
            else if (createType == CreateType.System)
            {
                string component = null;
                if (systemType >= ECSSystemGenerateType.GroupExecute && componentTypes != null)
                {
                    component = componentTypes[componentSelectIdx];
                }
                content = SystemGenertor.Gen(className, cfg.Name, systemType, component);
            }
            File.WriteAllText(filePath, content, System.Text.Encoding.UTF8);
            AssetDatabase.ImportAsset(filePath);
            if (OpenAfterCreate)
            {
                var obj = AssetDatabase.LoadMainAssetAtPath(filePath);
                if (obj)
                {
                    AssetDatabase.OpenAsset(obj);
                }
            }
        }

        static bool CreateECSCheck()
        {
            if (ECSConfig.Instance.Contexts.Count > 0)
            {
                return true;
            }
            EditorUtility.DisplayDialog("错误", "请先创建Context", "ok");
            ECSManagerWindow.Open();
            return false;
        }

        [MenuItem("ECS/创建Component")]
        [MenuItem("Assets/Create/ECS Component")]
        static void CreateECSComponent()
        {
            if (CreateECSCheck())
            {
                var wizard = DisplayWizard<ECSCreateWizard>("创建ECS文件");
                wizard.createType = CreateType.Component;
                wizard.UpdateContextSelect();
            }
        }

        [MenuItem("ECS/创建System")]
        [MenuItem("Assets/Create/ECS System")]
        static void CreateECSSystem()
        {
            if (CreateECSCheck())
            {
                var wizard = DisplayWizard<ECSCreateWizard>("创建ECS文件");
                wizard.createType = CreateType.System;
                wizard.UpdateContextSelect();
            }
        }

    }
}