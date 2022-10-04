using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrameLine
{
    public class FrameLineCollectorAsset<T> : ScriptableObject where T : FrameLineAsset
    {
        [SerializeField]
        private List<T> frameLines = new List<T>();

        public IReadOnlyList<T> FrameLines => frameLines;

        public void Add(T asset)
        {
            if (frameLines.Contains(asset))
                return;
            frameLines.Add(asset);
            AssetDatabase.AddObjectToAsset(asset, this);
            EditorUtility.SetDirty(this);
        }

        public void Remove(T asset)
        {
            if (!frameLines.Contains(asset))
                return;
            AssetDatabase.RemoveObjectFromAsset(asset);
            frameLines.Remove(asset);
            EditorUtility.SetDirty(this);
        }

        public void Destroy(T asset)
        {
            if (frameLines.Contains(asset))
            {
                AssetDatabase.RemoveObjectFromAsset(asset);
                frameLines.Remove(asset);
                EditorUtility.SetDirty(this);
            }
            Undo.ClearUndo(asset);
            DestroyImmediate(asset);
        }
    }
}
