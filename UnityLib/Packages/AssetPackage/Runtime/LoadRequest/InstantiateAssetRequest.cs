using UnityEngine;

namespace AssetPackage
{
    public abstract class InstantiateAssetRequest<T> : CustomYieldInstruction where T : Object
    {
        public T Asset { get; protected set; }
        public T OriginalAsset { get; protected set; }

        protected Transform parent;
        protected bool worldPositionStays;
        public abstract float Progeres { get; }
        private System.Action<T> onComplete;
        //仅操作完成时调用
        public event System.Action<T> OnComplete
        {
            add
            {
                if (Asset)
                {
                    value(Asset);
                }
                else
                {
                    onComplete += value;
                }
            }
            remove
            {
                onComplete -= value;
            }
        }
        private System.Action<T> onFinish;
        //失败的时候也会调用
        public event System.Action<T> OnFinish
        {
            add
            {
                if (!keepWaiting)
                {
                    value(Asset);
                }
                else
                {
                    onFinish += value;
                }
            }
            remove
            {
                onFinish += value;
            }
        }

        public InstantiateAssetRequest(Transform parent, bool worldPositionStays)
        {
            this.parent = parent;
            this.worldPositionStays = worldPositionStays;
        }

        public void SetAsset(T asset)
        {
            OriginalAsset = asset;
            Asset = Instantiate();
            DoLoadCallBack();
        }

        protected abstract T Instantiate();
        protected void DoLoadCallBack()
        {
            onFinish?.Invoke(Asset);
            if (Asset)
                onComplete?.Invoke(Asset);
            onFinish = null;
            onComplete = null;
        }
    }
}