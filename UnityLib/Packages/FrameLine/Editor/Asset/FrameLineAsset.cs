using UnityEngine;
namespace FrameLine
{
    public abstract class FrameLineAsset : ScriptableObject
    {
        public string Comment;
        public int FrameCount;
        [SerializeField, HideInInspector]
        private uint keyIndex;
        public string LoadTime { get; private set; }
        public ulong KeyIndex => keyIndex;
        protected virtual void OnEnable()
        {
            //文件被修改重新加载，会调用这里
            //在编辑的过程中可以根据LoadTime判断文件是否被从外部修改
            LoadTime = System.DateTime.Now.ToString();
        }


        public FrameClip AddClip(int groupId, int frame, IFrameLineClipData data)
        {
            var group = FindGroup(groupId);
            if (group == null) 
                return null;
            ulong id = (uint)groupId;
            id <<= 32;
            id |= ++keyIndex;
            FrameClip clip = new FrameClip
            {
                ID = id,
                StartFrame = frame,
                Name = GetTypeShowName(data.GetType())
            };
            clip.SetData(data);
            group.Clips.Add(clip);
            return clip;
        }

        public bool RemoveClip(FrameClipRef clip)
        {
            int groupId = clip.GroupId;
            var group = FindGroup(groupId);
            if (group != null)
            {
                var index = group.Clips.FindIndex(it => it.ID == clip.ID);
                if (index >= 0)
                {
                    group.Clips.RemoveAt(index);
                    return true;
                }
            }

            return false;
        }
        public static string GetTypeShowName(System.Type type)
        {
            return type.Name;
        }

        public FrameClip Find(ulong id)
        {
            int groupId = (int)(id >> 32);
            var group = FindGroup(groupId);
            if (group != null)
                return group.Clips.Find(it => it.ID == id);
            return null;
        }

        public abstract FrameClipGroup FindGroup(int id);

        public virtual FrameLineScene CreateScene()
        {
            var scene = CreateInstance<FrameLineScene>();
            scene.hideFlags = HideFlags.HideAndDontSave;
            return scene;
        }
    }
}