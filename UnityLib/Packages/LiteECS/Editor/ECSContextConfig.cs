using UnityEngine;
using System.IO;
using System.Collections.Generic;
namespace LiteECS.Editor
{
    [System.Serializable]
    public class ECSContextConfig
    {
        public string Name;
        public string UpLevelContext;
        public string DirectoryPath;

        private List<System.Type> _componentTypes;
        public List<System.Type> ComponentTypes
        {
            get
            {
                if (_componentTypes == null)
                {
                    _componentTypes = ECSContextUtils.GetAllComponentTypes(this);
                }
                return _componentTypes;
            }
        }

        public void GenECSFile(bool force)
        {
            var ecsFile = Path.Combine(DirectoryPath, $"{Name}ECS.cs");
            if (!File.Exists(ecsFile) || force)
            {
                var fileContext = ContextGenertor.Gen(Name, UpLevelContext);
                File.WriteAllText(ecsFile, fileContext, new System.Text.UTF8Encoding(false));
            }
        }

        public void InitECSDirectory()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(DirectoryPath))
            {
                Debug.LogError("名字和路径不对，初始化失败");
                return;
            }
            var dir = new DirectoryInfo(DirectoryPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            GenECSFile(false);

            string[] childrenDir = { "Components", "Systems", "Generates" };
            foreach (var child in childrenDir)
            {
                string path = Path.Combine(DirectoryPath, child);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

    }
}
