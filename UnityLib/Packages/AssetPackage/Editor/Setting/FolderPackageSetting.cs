using System.Collections.Generic;
using System.IO;

namespace AssetPackage
{
    //将文件夹打包
    public class FolderPackageSetting : AssetPackageSetting
    {
        public string Folder;
        //文件扩展名
        public List<string> Extensions = new List<string>();
        public override List<string> AssetPaths
        {
            get
            {
                List<string> assetPaths = new List<string>();
                if (!string.IsNullOrEmpty(Folder) && Folder.StartsWith("Assets/"))
                {
                    foreach (var ext in Extensions)
                    {
                        var files = Directory.GetFiles(Folder, "*" + ext, SearchOption.TopDirectoryOnly);
                        foreach (var file in files)
                        {
                            assetPaths.Add(file.Replace('\\', '/'));
                        }
                    }
                }
                assetPaths.Sort();
                return assetPaths;
            }
        }


        public override bool IsInPackage(string path)
        {
            if (!string.IsNullOrEmpty(Folder) && Folder.StartsWith("Assets/"))
            {
                string folder = Path.GetDirectoryName(path);
                if (folder == path)
                {
                    var extension = Path.GetExtension(path).ToLower();
                    if (Extensions.Contains(extension))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }


}