using UnityEngine;

namespace AssetPackage
{
    public abstract class AssetInstantiateRequest<T> : CustomYieldInstruction where T : Object
    {
        public T Asset { get; protected set; }
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

        protected Transform parent;
        protected bool worldPositionStays;

        public AssetInstantiateRequest(Transform parent = null, bool worldPositionStays = false)
        {
            this.parent = parent;
            this.worldPositionStays = worldPositionStays;
        }
        protected void DoLoadCallBack()
        {
            onFinish?.Invoke(Asset);
            if (Asset)
                onComplete?.Invoke(Asset);
        }
    }
}