using UnityEngine;

namespace AssetPackage
{
    internal class AssetDatabaseInstantiateAssetRequest<T> : InstantiateAssetRequest<T> where T : Object
    {
        public AssetDatabaseInstantiateAssetRequest(T asset, Transform parent = null, bool worldPositionStays = false) : base(parent, worldPositionStays)
        {
#if UNITY_EDITOR
            OriginalAsset = asset;
            if (asset)
            {
                if (asset is GameObject go)
                {
                    Asset = UnityEditor.PrefabUtility.InstantiatePrefab(asset, parent) as T;
                    if (worldPositionStays)
                    {
                        var trans = (Asset as GameObject).transform;
                        trans.position = go.transform.position;
                        trans.rotation = go.transform.rotation;
                        trans.localScale = go.transform.localScale;
                    }
                }
                else
                {
                    Asset = Object.Instantiate(asset);
                }
                   
            }
            DoLoadCallBack();
#endif
        }

        public override float Progeres => 1;

        public override bool keepWaiting => false;

    }
}