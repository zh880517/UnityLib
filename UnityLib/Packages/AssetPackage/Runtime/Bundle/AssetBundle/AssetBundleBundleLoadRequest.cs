using System.Collections.Generic;
using System.Linq;

namespace AssetPackage
{
    internal class AssetBundleBundleLoadRequest : BundleLoadRequest
    {
        const int MaxLoadCount = 40;
        public override float Progress
        {
            get
            {
                int count = finshCount + waitQueue.Count + loadingRequest.Count;
                float progress = finshCount;
                if (loadingRequest.Count > 0)
                    progress += loadingRequest.Sum(it=>it.Progress)/loadingRequest.Count;
                progress /= count;
                return progress;
            }
        }

        public override bool keepWaiting => waitQueue.Count > 0 || loadingRequest.Count > 0;

        private int finshCount;
        private List<LoadAssetBundleRequest> loadingRequest = new List<LoadAssetBundleRequest>();
        public Queue<AssetBundleInfo> waitQueue;
        public IAssetPathProvider pathProvider;

        public AssetBundleBundleLoadRequest(Queue<AssetBundleInfo> queue, IAssetPathProvider provider)
        {
            waitQueue = queue;
            pathProvider = provider;
        }

        public override bool OnTick()
        {
            for (int i = 0; i < loadingRequest.Count; ++i)
            {
                var request = loadingRequest[i];
                if (request.IsDone)
                {
                    request.OnFinish();
                    finshCount++;
                    if (waitQueue.Count > 0)
                    {
                        loadingRequest[i] = CreatLoadRequest(waitQueue.Dequeue());
                    }
                    else
                    {
                        loadingRequest.RemoveAt(i);
                        --i;
                    }
                }
            }
            while (loadingRequest.Count < MaxLoadCount && waitQueue.Count > 0)
            {
                loadingRequest.Add(CreatLoadRequest(waitQueue.Dequeue()));
            }
            return base.OnTick();
        }

        private LoadAssetBundleRequest CreatLoadRequest(AssetBundleInfo info)
        {
            string loadPath = pathProvider.GetAssetBundlePath(info.Name);
            if (loadPath.StartsWith("jar:") || loadPath.StartsWith("http"))//android平台的streamingAssetsPath，需要使用缓存方式加载
            {
                return new HttpLoadAssetBundleRequest(loadPath, info.Hash);
            }

            return new FileLoadAssetBundleRequest(loadPath);
        }

    }


}