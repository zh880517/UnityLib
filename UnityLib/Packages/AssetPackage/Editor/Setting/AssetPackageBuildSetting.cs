using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace AssetPackage
{
    public class AssetPackageBuildSetting : ScriptableObject
    {
        public const string SettingFolder = "Assets/PackageSetting/";
        private static AssetPackageBuildSetting _Instance;
        public static AssetPackageBuildSetting Instance
        {
            get
            {
                if (_Instance == null)
                {
                    string path = SettingFolder + "AssetPackageBuildSetting.asset";
                    _Instance = AssetDatabase.LoadAssetAtPath<AssetPackageBuildSetting>(path);
                    if (_Instance == null)
                    {
                        _Instance = CreateInstance<AssetPackageBuildSetting>();
                        _Instance.Packages = new List<AssetPackageSetting>();
                        AssetDatabase.CreateAsset(_Instance, path);
                    }
                }
                return _Instance;
            }
        }
        //压缩方式
        public bool CompressedAssetBundle = true;
        public List<AssetPackageSetting> Packages;


    }
}
