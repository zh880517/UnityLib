using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
namespace MeshPainter
{
    public class MeshPainterConfigEditor : EditorWindow
    {
        public SerializedObject serializedObject;
        public Shader addShader;
        public DefaultAsset selectFolder;
        public int[] channelNames = new int[4];
        public int controlName;
        public string[] textureParams;

        public Vector2 scrollPos;
        private void OnGUI()
        {
            using (var scroll = new GUILayout.ScrollViewScope(scrollPos))
            {
                serializedObject = serializedObject ?? new SerializedObject(MeshPainterConfig.Instance);
                serializedObject.UpdateIfRequiredOrScript();
                SerializedProperty iterator = serializedObject.GetIterator();
                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                    {
                        EditorGUILayout.PropertyField(iterator, true);
                    }
                    enterChildren = false;
                }
                serializedObject.ApplyModifiedProperties();
                using (new GUILayout.HorizontalScope())
                {
                    selectFolder = EditorGUILayout.ObjectField("画刷文件夹", selectFolder, typeof(DefaultAsset), false) as DefaultAsset;
                    if (selectFolder && GUILayout.Button("添加画刷"))
                    {
                        var path = AssetDatabase.GetAssetPath(selectFolder);
                        string errorMsg = null;
                        do
                        {
                            if (string.IsNullOrWhiteSpace(path))
                                break;
                            if (Path.HasExtension(path))
                            {
                                errorMsg = "请选择文件夹";
                                break;
                            }
                            List<Texture2D> textures = new List<Texture2D>();
                            var guids = AssetDatabase.FindAssets("t:texture2D", new string[] { path });
                            foreach (var id in guids)
                            {
                                string filePath = AssetDatabase.GUIDToAssetPath(id);
                                int idx = filePath.LastIndexOf('/');
                                if (idx > path.Length)
                                    continue;
                                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                                if (texture && !MeshPainterConfig.Instance.Brushs.Contains(texture))
                                {
                                    textures.Add(texture);
                                }

                            }
                            if (textures.Count > 0 || MeshPainterConfig.Instance.Brushs.Exists(it => !it))
                            {
                                Undo.RegisterCompleteObjectUndo(MeshPainterConfig.Instance, "add brush");
                                EditorUtility.SetDirty(MeshPainterConfig.Instance);
                                MeshPainterConfig.Instance.Brushs.RemoveAll(it => !it);
                                MeshPainterConfig.Instance.Brushs.AddRange(textures);
                            }
                        } while (false);
                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            EditorUtility.DisplayDialog("提示", errorMsg, "ok");
                        }
                    }

                }

                GUILayout.Label("选择需要添加的材质球");
                EditorGUI.BeginChangeCheck();
                addShader = EditorGUILayout.ObjectField(addShader, typeof(Shader), false) as Shader;
                if (EditorGUI.EndChangeCheck() && addShader)
                {
                    channelNames = new int[4];
                    controlName = 0;
                    List<string> paramNames = new List<string>() { "无" };
                    for (int i = 0; i < addShader.GetPropertyCount(); ++i)
                    {
                        if (addShader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Texture)
                        {
                            paramNames.Add(addShader.GetPropertyName(i));
                        }
                    }
                    textureParams = paramNames.ToArray();
                }
                if (addShader && MeshPainterConfig.Instance.FindShader(addShader) == null)
                {
                    channelNames[0] = EditorGUILayout.Popup("通道0", channelNames[0], textureParams);
                    channelNames[1] = EditorGUILayout.Popup("通道1", channelNames[1], textureParams);
                    channelNames[2] = EditorGUILayout.Popup("通道2", channelNames[2], textureParams);
                    channelNames[3] = EditorGUILayout.Popup("通道3", channelNames[3], textureParams);
                    controlName = EditorGUILayout.Popup("控制贴图", controlName, textureParams);
                    if (channelNames[0] == 0 || channelNames[1] == 0 || controlName == 0)
                    {
                        GUILayout.Label("需要选择控制贴图以及至少两个通道");
                    }
                    else
                    {
                        if (GUILayout.Button("添加"))
                        {
                            MeshPainterShader shader = new MeshPainterShader();
                            shader.Shader = addShader;
                            for (int i = 0; i < 4; ++i)
                            {
                                if (channelNames[i] == 0)
                                    break;
                                shader.ChannleName.Add(textureParams[channelNames[i]]);
                            }
                            shader.ControlName = textureParams[controlName];
                            Undo.RegisterCompleteObjectUndo(MeshPainterConfig.Instance, "add shader");
                            EditorUtility.SetDirty(MeshPainterConfig.Instance);
                            MeshPainterConfig.Instance.Shaders.Add(shader);
                        }

                    }
                }

                scrollPos = scroll.scrollPosition;
            }


        }
    }
}