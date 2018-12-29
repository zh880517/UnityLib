using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleLoader
{
    public class LoaderBehaviour : MonoBehaviour
    { }

    public static LoaderBehaviour loaderBehaviour;

    class Loader
    {
        public Coroutine Coro;
        public int Index;
    }

    public bool CacheLoad { get; private set; }
    private readonly Dictionary<AssetBundleInfo, List<CountYield>> loadCounterMap = new Dictionary<AssetBundleInfo, List<CountYield>>();
    private readonly Queue<AssetBundleInfo> waiteQueue = new Queue<AssetBundleInfo>();
    private readonly Loader[] loaderList;
    public AssetBundleLoader(uint queueCount, bool useCache = true)
    {
        loaderList = new Loader[queueCount];
        CacheLoad = useCache;
        if (loaderBehaviour == null)
        {
            GameObject go = new GameObject("_AssetBundleLoader_");
            loaderBehaviour = go.AddComponent<LoaderBehaviour>();
            Object.DontDestroyOnLoad(go);
        }
    }

    public void UseCacheLoad(bool useCache)
    {
        CacheLoad = useCache;
    }

    public void Load(ICollection<AssetBundleInfo> infoList, CountYield countYield)
    {
        foreach (var info in infoList)
        {
            if (info.Bundle != null)
            {
                countYield.CompleteCount++;
                continue;
            }
            List<CountYield> countYields;
            if (!loadCounterMap.TryGetValue(info, out countYields))
            {
                countYields = new List<CountYield>();
                loadCounterMap.Add(info, countYields);
                waiteQueue.Enqueue(info);
            }
            countYields.Add(countYield);
        }
        CheckLoad();
    }
    
    public IEnumerator LoadYield(ICollection<AssetBundleInfo> infoList, CountYield countYield = null)
    {
        if (countYield == null)
            countYield = new CountYield(infoList.Count);

        foreach (var info in infoList)
        {
            if (info.Bundle != null)
            {
                countYield.CompleteCount++;
                continue;
            }
            List<CountYield> countYields;
            if (!loadCounterMap.TryGetValue(info, out countYields))
            {
                countYields = new List<CountYield>();
                loadCounterMap.Add(info, countYields);
                waiteQueue.Enqueue(info);
            }
            countYields.Add(countYield);
        }
        CheckLoad();
        yield return countYield;
    }
    public void Clear()
    {
        loaderBehaviour.StopAllCoroutines();
        loadCounterMap.Clear();
        waiteQueue.Clear();
        for (int i = 0; i < loaderList.Length; ++i)
        {
            if (loaderList[i] != null)
                loaderList[i].Coro = null;
        }
    }

    private void CheckLoad()
    {
        if (waiteQueue.Count == 0)
            return;
        int idleCount = 0;
        for (int i = 0; i < loaderList.Length; ++i)
        {
            if (loaderList[i] == null)
            {
                loaderList[i] = new Loader { Index = i };
            }
            if (loaderList[i].Coro == null)
            {
                idleCount++;
                if (idleCount > waiteQueue.Count)
                    break;
                loaderList[i].Coro = loaderBehaviour.StartCoroutine(DoLoad(i));
            }
        }
    }

    private IEnumerator DoLoad(int index)
    {
        while (true)
        {
            if (waiteQueue.Count == 0)
            {
                loaderList[index].Coro = null;
                yield break;
            }
            var info = waiteQueue.Dequeue();
            if (CacheLoad)
                yield return LoadByCache(info);
            if (!CacheLoad)
            {
                if (info.Bundle == null && info.Stage == AssetBundleInfo.LoadStage.CacheLoaded)
                    info.Stage = AssetBundleInfo.LoadStage.None;
                yield return LoadNoneCache(info);
            }
            //清除crc信息，防止下次加载再进行crc校验
            info.Crc = 0;
            List<CountYield> listCount = null;
            if (loadCounterMap.TryGetValue(info, out listCount))
            {
                loadCounterMap.Remove(info);
                foreach (var ct in listCount)
                {
                    ct.CompleteCount++;
                }
            }
        }
    }
    
    //通过缓存方式加载，会额外占用一部分存储空间
    //适合android或者Assetbundle打包时候默认压缩方式（高压缩比）
    private IEnumerator LoadByCache(AssetBundleInfo info)
    {
        if (info.Stage == AssetBundleInfo.LoadStage.None)
        {
            /*
             * 缓存方式加载Assetbundle，如果Hash128不对，但是目标文件存在，依然会加载
             * 并且在缓存中更新资源的Hash128为加载时候提供的Hash128
             * 如果热更新提前更新了AssetBundleManifest，但是AssetBundle没有更新，那么下次更新了Assetbundle后
             * 加载的时候发现Hash128没有更新，就直接存缓存中加载，不会从提供的url中加载（除非url改变了）
             * 所以这个时候需要加上crc参数（打包Assetbundle结束的时候获取然后记录下来）
             * crc方式加载速度比较慢，可以在crc加载一次就删除本地的crc数据，每次热更新后更新crc数据的时候再用crc方式加载一次
             */
            string url = PathUtil.GetStreamAssetFileUrl(info.Name);
            info.Stage = AssetBundleInfo.LoadStage.CacheLoading;
            WWW download = WWW.LoadFromCacheOrDownload(url, info.Hash, info.Crc);
            yield return download;
            if (string.IsNullOrEmpty(download.error))
                info.Bundle = download.assetBundle;
            if (info.Bundle == null && info.Crc != 0)
            {
                //如果用crc校验加载失败，再尝试用不用crc加载
                download = WWW.LoadFromCacheOrDownload(url, info.Hash, 0);
                yield return download;
                if (string.IsNullOrEmpty(download.error))
                    info.Bundle = download.assetBundle;
            }
            info.Stage = AssetBundleInfo.LoadStage.CacheLoaded;
            if (info.Bundle == null)
                Debug.LogError("AssetBundle 加载失败 => " + info.Name);
        }
    }

    //非换缓存方式加载，适合非安卓平台并且在Assetbundle打包的时候选择不压缩或者LZ4压缩方式的（默认压缩方式需要先解压再加载比较慢）
    private IEnumerator LoadNoneCache(AssetBundleInfo info)
    {
        if (info.Stage == AssetBundleInfo.LoadStage.None)
        {
            info.Stage = AssetBundleInfo.LoadStage.NoneCacheLoading;
            string path = PathUtil.GetStreamAssetFilePath(info.Name);
            var request = AssetBundle.LoadFromFileAsync(path);
            yield return request;
            info.Bundle = request.assetBundle;
            info.Stage = AssetBundleInfo.LoadStage.NoneCacheLoaded;
            if (info.Bundle == null)
                Debug.LogError("AssetBundle 加载失败 => " + info.Name);
        }
        
    }

    
}
