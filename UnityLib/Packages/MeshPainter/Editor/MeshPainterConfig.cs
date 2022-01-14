using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
namespace MeshPainter
{
    [System.Serializable]
    public class MeshPainterShader
    {
        public Shader Shader;
        public List<string> ChannleName = new List<string>();
        public string ControlName;
    }


    public class MeshPainterConfig : ScriptableObject
    {
        public MeshPainterConfig()
        {
            _instance = this;
        }

        const string savepath = "Assets/Editor/MeshPainterConfig.asset";
        private static MeshPainterConfig _instance;
        public static MeshPainterConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = AssetDatabase.LoadAssetAtPath<MeshPainterConfig>(savepath);
                    if (_instance == null)
                    {
                        _instance = CreateInstance<MeshPainterConfig>();
                        _instance.hideFlags = HideFlags.HideInInspector;
                        var dir = Path.GetDirectoryName(savepath);
                        if (Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        AssetDatabase.CreateAsset(_instance, savepath);
                    }
                }
                return _instance;
            }
        }
        public List<Texture2D> Brushs = new List<Texture2D>();
        public List<MeshPainterShader> Shaders = new List<MeshPainterShader>();


        public MeshPainterShader FindShader(Shader shader)
        {
            return Shaders.Find(it => it.Shader == shader);
        }
    }

}