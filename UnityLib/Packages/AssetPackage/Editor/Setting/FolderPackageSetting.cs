using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetPackage
{
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
                foreach (var ext in Extensions)
                {
                    var files = Directory.GetFiles(Folder, "*" + ext, SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        assetPaths.Add(file.Replace('\\', '/'));
                    }
                }
                return assetPaths;
            }
        }


        public override bool IsInPackage(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            string folder = Path.GetDirectoryName(path);
            if (folder == path)
            {
                var extension = Path.GetExtension(path).ToLower();
                if (Extensions.Contains(extension))
                {
                    return true;
                }
            }
            return false;
        }
    }


}