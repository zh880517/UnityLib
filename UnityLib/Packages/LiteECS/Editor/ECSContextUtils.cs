using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;

namespace LiteECS.Editor
{
    public static class ECSContextUtils
    {
        public static void GeneratorComponent(ECSContextConfig config)
        {
            string folder = Path.Combine(config.DirectoryPath, "Generates");
            GeneratorFolder generatorFolder = new GeneratorFolder(folder, "cs");
            ComponentGenerator componentGenerator = new ComponentGenerator(config);
            componentGenerator.Gen(generatorFolder);
            generatorFolder.WriteFile();
        }

        [MenuItem("ECS/生成所有")]
        public static void GenAllContextComponent()
        {
            foreach (var contxt in ECSConfig.Instance.Contexts)
            {
                GeneratorComponent(contxt);
            }
            AssetDatabase.Refresh();
        }

        public static Assembly GetECSAssembly(ECSContextConfig config)
        {
            string contextFile = Path.Combine(config.DirectoryPath, $"{config.Name}ECS.cs");
            return MonoScriptUtils.GetScriptAssembly(contextFile);
        }

        public static List<Type> GetAllComponentTypes(ECSContextConfig config)
        {
            Assembly assembly = GetECSAssembly(config);
            if (assembly == null)
                return null;

            Type baseInterface = assembly.GetType($"I{config.Name}Component");
            if (baseInterface == null)
                return null;

            List<Type> types = new List<Type>();
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;
                if (baseInterface.IsAssignableFrom(type))
                {
                    types.Add(type);
                }
            }
            return types;
        }

        public static List<string> GetComponentCatalog(this ECSContextConfig config)
        {
            List<string> results = new List<string>();
            string path = Path.Combine(config.DirectoryPath, "Components");
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            AddDir(directoryInfo, null, results);
            return results;
        }

        public static List<string> GetSystemsCatalog(this ECSContextConfig config)
        {
            List<string> results = new List<string>();
            string path = Path.Combine(config.DirectoryPath, "Systems");
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            AddDir(directoryInfo, null, results);
            return results;
        }

        private static void AddDir(DirectoryInfo directoryInfo, string name, List<string> names)
        {
            if (directoryInfo.Exists)
            {
                string addName = directoryInfo.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    addName = $"{name}/{addName}";
                }
                names.Add(addName);
                var children = directoryInfo.GetDirectories();
                foreach (var child in children)
                {
                    AddDir(child, addName, names);
                }
            }
        }

    }

}
