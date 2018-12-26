using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssertBundleLoader
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
    private readonly Dictionary<AssertBundleInfo, List<CountYield>> loadCounterMap = new Dictionary<AssertBundleInfo, List<CountYield>>();
    private readonly Queue<AssertBundleInfo> waiteQueue = new Queue<AssertBundleInfo>();
    private readonly Loader[] loaderList;
    public AssertBundleLoader(uint queueCount, bool useCache = true)
    {
        loaderList = new Loader[queueCount];
        CacheLoad = useCache;
        if (loaderBehaviour == null)
        {
            GameObject go = new GameObject("_AssertBundleLoader_");
            loaderBehaviour = go.AddComponent<LoaderBehaviour>();
            Object.DontDestroyOnLoad(go);
        }
    }

    public void UseCacheLoad(bool useCache)
    {
        CacheLoad = useCache;
    }

    public void Load(ICollection<AssertBundleInfo> infoList, CountYield countYield)
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
    
    public IEnumerator LoadYield(ICollection<AssertBundleInfo> infoList, CountYield countYield = null)
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
                if (info.Bundle == null && info.Stage == AssertBundleInfo.LoadStage.CacheLoaded)
                    info.Stage = AssertBundleInfo.LoadStage.None;
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
    //适合android或者Assertbundle打包时候默认压缩方式（高压缩比）
    private IEnumerator LoadByCache(AssertBundleInfo info)
    {
        if (info.Stage == AssertBundleInfo.LoadStage.None)
        {
            /*
             * 缓存方式加载Assertbundle，如果Hash128不对，但是目标文件存在，依然会加载
             * 并且在缓存中更新资源的Hash128为加载时候提供的Hash128
             * 如果热更新提前更新了AssetBundleManifest，但是AssertBundle没有更新，那么下次更新了Assertbundle后
             * 加载的时候发现Hash128没有更新，就直接存缓存中加载，不会从提供的url中加载（除非url改变了）
             * 所以这个时候需要加上crc参数（打包Assertbundle结束的时候获取然后记录下来）
             * crc方式加载速度比较慢，可以在crc加载一次就删除本地的crc数据，每次热更新后更新crc数据的时候再用crc方式加载一次
             */
            string url = PathUtil.GetStreamAssertFileUrl(info.Name);
            info.Stage = AssertBundleInfo.LoadStage.CacheLoading;
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
            info.Stage = AssertBundleInfo.LoadStage.CacheLoaded;
            if (info.Bundle == null)
                Debug.LogError("AssertBundle 加载失败 => " + info.Name);
        }
        yield return null;
    }

    //非换缓存方式加载，适合非安卓平台并且在Assertbundle打包的时候选择不压缩或者LZ4M压缩方式的（默认压缩方式需要先解压再加载比较慢）
    private IEnumerator LoadNoneCache(AssertBundleInfo info)
    {
        if (info.Stage == AssertBundleInfo.LoadStage.None)
        {
            info.Stage = AssertBundleInfo.LoadStage.NoneCacheLoading;
            string path = PathUtil.GetStreamAssertFilePath(info.Name);
            var request = AssetBundle.LoadFromFileAsync(path);
            yield return request;
            info.Bundle = request.assetBundle;
            info.Stage = AssertBundleInfo.LoadStage.NoneCacheLoaded;
            if (info.Bundle == null)
                Debug.LogError("AssertBundle 加载失败 => " + info.Name);
        }
        
    }

    
}
