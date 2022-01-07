using UnityEngine;
namespace AssetPackage
{
    public abstract class LoadAssetRequest<T> : CustomYieldInstruction where T : Object
    {
        public T Asset { get; protected set; }

        public abstract float Progeres { get; }

        //仅操作完成时调用
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
        //失败的时候也会调用
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
                    onComplete += value;
                }
            }
            remove
            {
                onFinish += value;
            }
        }

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