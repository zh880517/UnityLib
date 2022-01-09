using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace AssetPackage
{
    //零散文件打包
    public class FilesPackageSetting : AssetPackageSetting
    {
        public List<Object> Assets = new List<Object>();
        public override List<string> AssetPaths => Assets.Select(it=>AssetDatabase.GetAssetPath(it)).ToList();


        public override bool IsInPackage(string path)
        {
            var obj = AssetDatabase.LoadMainAssetAtPath(path);
            return Assets.Contains(obj);
        }
    }

}
