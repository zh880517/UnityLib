using System.Collections.Generic;
using UnityEngine;
namespace FrameLine
{
    public class FrameLineAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        public string Comment;
        public int FrameCount;
        [SerializeField, HideInInspector]
        private ulong keyIndex;
        [SerializeField, HideInInspector]
        protected List<FrameClip> clips = new List<FrameClip>();

        public IReadOnlyList<FrameClip> Clips => clips;
        public string LoadTime { get; private set; }
        public ulong KeyIndex => keyIndex;
        protected virtual void OnEnable()
        {
            //文件被修改重新加载，会调用这里
            //在编辑的过程中可以根据LoadTime判断文件是否被从外部修改
            LoadTime = System.DateTime.Now.ToString();
        }

        public FrameClip AddClip(int frame, IFrameLineClipData data)
        {
            FrameClip clip = new FrameClip
            {
                ID = ++keyIndex,
                StartFrame = frame,
                Name = GetTypeShowName(data.GetType())
            };
            clip.SetData(data);
            clips.Add(clip);
            return clip;
        }

        public bool RemoveClip(FrameClipRef clip)
        {
            var index = clips.FindIndex(it => it.ID == clip.ID);
            if (index >= 0)
            {
                clips.RemoveAt(index);
                return true;
            }
            return false;
        }
        public static string GetTypeShowName(System.Type type)
        {
            return type.Name;
        }

        public FrameClip Find(ulong id)
        {
            return clips.Find(it => it.ID == id);
        }

        public virtual FrameLineScene CreateScene()
        {
            var scene = CreateInstance<FrameLineScene>();
            scene.hideFlags = HideFlags.HideAndDontSave;
            return scene;
        }

        public virtual void OnBeforeSerialize()
        {
        }

        public virtual void OnAfterDeserialize()
        {
        }
    }
}