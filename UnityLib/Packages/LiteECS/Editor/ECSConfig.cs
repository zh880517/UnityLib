using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LiteECS.Editor
{
    [System.Serializable]
    public class ECSConfig
    {
        [SerializeField]
        private int modifyVersion;
        public List<ECSContextConfig> Contexts;

        private static ECSConfig _instance;

        const string SaveFile = "ProjectSettings/ECSConfig.json";

        public static ECSConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (File.Exists(SaveFile))
                    {
                        _instance = JsonUtility.FromJson<ECSConfig>(File.ReadAllText(SaveFile));
                    }
                    else
                    {
                        _instance = new ECSConfig { Contexts = new List<ECSContextConfig>() };
                        SaveToFile();
                    }
                }
                return _instance;
            }
        }
        protected ECSConfig()
        {
            _instance = this;
        }


        public ECSContextConfig AddContext(string name, string path)
        {
            do
            {
                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(path))
                {
                    if (Contexts.Exists(it => it.Name == name))
                    {
                        EditorUtility.DisplayDialog("错误", $"已经存在重名的Context => {name}", "ok");
                        break;
                    }
                    if (Contexts.Exists(it => it.DirectoryPath == path))
                    {
                        EditorUtility.DisplayDialog("错误", $"已经存在相同的文件夹 => {path}", "ok");
                        break;
                    }
                    if (!char.IsUpper(name[0]))
                    {
                        EditorUtility.DisplayDialog("错误", "名字首字母应该大写", "ok");
                        break;
                    }
                    bool validName = true;
                    for (int i = 0; i < name.Length; ++i)
                    {
                        if (!char.IsLetter(name[i]))
                        {
                            EditorUtility.DisplayDialog("错误", $"名字含有非法字符{name[i]}", "ok");
                            break;
                        }
                    }
                    if (!validName)
                        break;
                    MakeModify("add ecs from exits");
                    ECSContextConfig config = new ECSContextConfig { Name = name, DirectoryPath = path };
                    Contexts.Add(config);
                    return config;
                }
            } while (false);
            return null;
        }


        public static void SaveToFile()
        {
            if (_instance == null)
                return;
            string directoryName = Path.GetDirectoryName(SaveFile);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            Instance.modifyVersion = 0;
            File.WriteAllText(SaveFile, JsonUtility.ToJson(_instance, true));
        }

        public static bool IsModfy()
        {
            return Instance.modifyVersion != 0;
        }

        public static void MakeModify(string name)
        {
            ++Instance.modifyVersion;
        }

        public static void ReloadFormFile()
        {
            _instance = JsonUtility.FromJson<ECSConfig>(SaveFile);
        }
    }
}
