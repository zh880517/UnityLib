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
                return loadingRequest.Sum(it=>it.Progress)/(loadingRequest.Count + waitQueue.Count);
            }
        }

        public override bool keepWaiting => waitQueue.Count > 0 || loadingRequest.Count > finshCount;

        private int finshCount;
        private List<LoadAssetBundleRequest> loadingRequest;
        public Queue<AssetBundleInfo> waitQueue;

        public AssetBundleBundleLoadRequest(Queue<AssetBundleInfo> queue)
        {
            waitQueue = queue;
            loadingRequest = new List<LoadAssetBundleRequest>(queue.Count);
        }

        public override bool OnTick()
        {
            finshCount = 0;
            for (int i = 0; i < loadingRequest.Count; ++i)
            {
                var request = loadingRequest[i];
                if (request.IsDone)
                {
                    finshCount++;
                }
            }
            while ((loadingRequest.Count - finshCount) < MaxLoadCount && waitQueue.Count > 0)
            {
                loadingRequest.Add(CreatLoadRequest(waitQueue.Dequeue()));
            }
            if (loadingRequest.Count == finshCount && waitQueue.Count == 0)
            {//等所有的AssetBundle都加载完成再处理，防止因为依赖关系导致加载资源时出问题
                foreach (var requet in loadingRequest)
                {
                    requet.OnFinish();
                }
            }
            return base.OnTick();
        }

        private LoadAssetBundleRequest CreatLoadRequest(AssetBundleInfo info)
        {
            string loadPath = AssetManager.PathProvider.GetAssetBundlePath(info.Name);
            if (loadPath.StartsWith("jar:") || loadPath.StartsWith("http"))//android平台的streamingAssetsPath，需要使用缓存方式加载
            {
                return new HttpLoadAssetBundleRequest(loadPath, info);
            }

            return new FileLoadAssetBundleRequest(loadPath, info);
        }

    }


}