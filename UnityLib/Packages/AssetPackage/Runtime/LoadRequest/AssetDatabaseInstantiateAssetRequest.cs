using UnityEngine;

namespace AssetPackage
{
    internal class AssetDatabaseInstantiateAssetRequest<T> : InstantiateAssetRequest<T> where T : Object
    {
        public AssetDatabaseInstantiateAssetRequest(T asset, Transform parent = null, bool worldPositionStays = false) : base(parent, worldPositionStays)
        {
            SetAsset(asset);
        }

        public override float Progeres => 1;

        public override bool keepWaiting => false;

        protected override T Instantiate()
        {
#if UNITY_EDITOR
            if (OriginalAsset is GameObject go)
            {
                var asset = UnityEditor.PrefabUtility.InstantiatePrefab(OriginalAsset, parent) as T;
                if (worldPositionStays)
                {
                    var trans = (Asset as GameObject).transform;
                    trans.position = go.transform.position;
                    trans.rotation = go.transform.rotation;
                    trans.localScale = go.transform.localScale;
                }
                return asset;
            }
#endif
            return Object.Instantiate<T>(OriginalAsset, parent, worldPositionStays);
        }
    }
}