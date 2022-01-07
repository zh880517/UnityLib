using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace AssetPackage
{
    public class FilesPackageSetting : AssetPackageSetting
    {
        public List<Object> Assets = new List<Object>();
        public override List<string> AssetPaths => Assets.Select(it=>AssetDatabase.GetAssetPath(it)).ToList();


        public override bool IsInPackage(Object obj)
        {
            return Assets.Contains(obj);
        }
    }

}
