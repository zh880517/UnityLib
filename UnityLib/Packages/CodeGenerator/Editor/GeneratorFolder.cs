using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeGenerator
{
    public class GeneratorFolder
    {
        private Dictionary<string, string> FileContents = new Dictionary<string, string>();
        public string DirectoryPath { get; private set; }
        private static UTF8Encoding encoding = new UTF8Encoding(false);
        private System.Func<string, bool> ignoreFileMatch;
        private string externName;

        public GeneratorFolder(string path, string externName, System.Func<string, bool> ignoreFileMatch = null)
        {
            DirectoryPath = path;
            if (!path.EndsWith("/"))
                DirectoryPath += "/";
            this.ignoreFileMatch = ignoreFileMatch;
            this.externName = externName;
        }

        public void AddFile(string name, string content)
        {
            FileContents[$"{name}.{externName}"] = content;
        }

        public void WriteFile()
        {
            var dirInfo = new DirectoryInfo(DirectoryPath);
            if (dirInfo.Exists)
            {
                var files = dirInfo.GetFiles(string.Format("*.{0}", externName));
                foreach (var file in files)
                {
                    if (!FileContents.ContainsKey(file.Name) && (ignoreFileMatch == null || !ignoreFileMatch(file.Name)))
                    {
                        file.Delete();
                    }
                }
            }
            else
            {
                dirInfo.Create();
            }
            foreach (var kv in FileContents)
            {
                string filePath = string.Format("{0}{1}", DirectoryPath, kv.Key);
                FileUtil.WriteFile(filePath, kv.Value);
            }
        }

    }
}