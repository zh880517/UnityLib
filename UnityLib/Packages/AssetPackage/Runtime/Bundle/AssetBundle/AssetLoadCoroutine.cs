using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetPackage
{
    public interface ILoadTick
    {
        bool OnTick();
    }

    internal class AssetLoadCoroutine : MonoBehaviour
    {
        private static AssetLoadCoroutine _instance;
        public static AssetLoadCoroutine Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("_AssetLoadCoroutine_");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<AssetLoadCoroutine>();
                }
                return _instance;
            }
        }

        private List<ILoadTick> loadQueue = new List<ILoadTick>();

        public void AddLoadTick(ILoadTick load)
        {
            loadQueue.Add(load);
            if (loadQueue.Count == 1)
                StartCoroutine(DoLoadTick());
        }

        private IEnumerator DoLoadTick()
        {
            for (int i=0; i <loadQueue.Count; ++i )
            {
                var loader = loadQueue[i];
                if (loader.OnTick())
                {
                    loadQueue.RemoveAt(i);
                    --i;
                }
            }
            if (loadQueue.Count > 0)
                yield return null;//下一帧继续执行
        }

        private void OnDestroy()
        {
            loadQueue.Clear();
            _instance = null;
        }
    }

}
