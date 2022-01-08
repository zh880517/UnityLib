using System.Collections.Generic;
using System.IO;
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
        public string BundleExtension = ".ab";
        //压缩方式
        public bool CompressedAssetBundle = true;
        public List<AssetPackageSetting> Packages;

        public Dictionary<string, string> GetAssets()
        {
            Dictionary<string, string> assets = new Dictionary<string, string>();
            foreach (var package in Packages)
            {
                if (!package.Enable)
                    continue;
                foreach (var path in package.AssetPaths)
                {
                    string name = string.Format("{0}/{1}", package.name, Path.GetFileNameWithoutExtension(path));
                    assets.Add(name, path);
                }
            }
            return assets;
        }

        public PackageManifest ToManifest()
        {
            PackageManifest manifest = new PackageManifest();
            foreach (var package in Packages)
            {
                if (!package.Enable)
                    continue;
                PackageManifest.BundleInfo bundleInfo = new PackageManifest.BundleInfo();
                bundleInfo.Name = ToAssetBundlName(package.name);
                foreach (var path in package.AssetPaths)
                {
                    string fileName = Path.GetFileNameWithoutExtension(path);
                    string assetName = string.Format("{0}/{1}", package.name, Path.GetFileNameWithoutExtension(fileName));
                    int location = manifest.Bundles.Count << 16 | bundleInfo.Assets.Count;
                    bundleInfo.Assets.Add(fileName);
                    manifest.Assets.Add(new PackageManifest.AssetInfo { Name = assetName, Location = location });
                }
                manifest.Bundles.Add(bundleInfo);
            }
            return manifest;
        }

        public string ToAssetBundlName(string name)
        {
            return name.Replace(' ', '^').ToLower();
        }

        public bool Check()
        {
            Dictionary<string, string> packageFiles = new Dictionary<string, string>();
            foreach (var package in Packages)
            {
                if (!package.Enable)
                    continue;
                foreach (var path in package.AssetPaths)
                {
                    if (packageFiles.TryGetValue(path, out string packName))
                    {
                        Debug.LogErrorFormat("资源{0}重复存在{1} 和 {2}", path, packName, package.name);
                        return false;
                    }
                    packageFiles.Add(path, package.name);
                }
            }
            return true;
        }

        private static HashSet<string> NonePackFiles = new HashSet<string>
        {
            ".cs", ".dll", ".so"
        };

        public bool Build(string outPutPath, IDependenciesPacakageProvider provider, BuildTarget targetPlatform, bool clearRemovedBundle)
        {
            if (!Check())
                return false;
            if (!outPutPath.EndsWith("/") || !outPutPath.EndsWith("\\"))
                outPutPath += "/";
            string bundlePath = outPutPath + "bundle/";
            
            List<string> packageFiles = new List<string>();
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            foreach (var package in Packages)
            {
                if (!package.Enable)
                    continue;
                var filePaths = package.AssetPaths;
                AssetBundleBuild build = new AssetBundleBuild
                {
                    assetBundleName = ToAssetBundlName(package.name),
                    assetNames = filePaths.ToArray()
                };
                packageFiles.AddRange(filePaths);
                builds.Add(build);
            }
            HashSet<string> dependencies = new HashSet<string>();
            foreach (var file in packageFiles)
            {
                var depenFiles = AssetDatabase.GetDependencies(file);
                foreach (var dep in depenFiles)
                {
                    var extension = Path.GetExtension(dep).ToLower();

                    if (!string.IsNullOrEmpty(extension) && !NonePackFiles.Contains(extension))
                    {
                        dependencies.Add(dep);
                    }
                }
            }
            //依赖文件分包
            if (provider != null)
            {
                var depBundles = provider.FilesToPackage(dependencies);
                if (depBundles != null)
                    builds.AddRange(depBundles);
            }
            for (int i=0; i< builds.Count; ++i)
            {
                string name = builds[i].assetBundleName;
                if (!name.EndsWith(BundleExtension))
                {
                    name += BundleExtension;
                    var info = builds[i];
                    info.assetBundleName = name;
                    builds[i] = info;
                }
            }
            BuildAssetBundleOptions bundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
            if (CompressedAssetBundle)
                bundleOptions = bundleOptions | BuildAssetBundleOptions.ChunkBasedCompression;
            var resutlt = BuildPipeline.BuildAssetBundles(bundlePath, builds.ToArray(), bundleOptions, targetPlatform);
            if (resutlt)
            {
                PackageManifest packageManifest = ToManifest();
                string json = JsonUtility.ToJson(packageManifest);
                File.WriteAllText(Path.Combine(bundlePath, "PackageManifest.json"), json, new System.Text.UTF8Encoding(false));
                if(clearRemovedBundle)
                {
                    HashSet<string> bundles = new HashSet<string>(resutlt.GetAllAssetBundlesWithVariant());
                    var files = Directory.GetFiles(bundlePath, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        if (!fileName.Contains(BundleExtension) || fileName.EndsWith(".meta") || fileName.EndsWith(".Manifest"))
                            continue;
                        string partName = fileName.Replace(bundlePath, "");
                        if (!bundles.Contains(partName))
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
            return resutlt != null;
        }

    }
}
