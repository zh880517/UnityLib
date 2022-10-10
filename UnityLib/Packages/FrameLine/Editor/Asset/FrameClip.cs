using System;

namespace FrameLine
{
    [Serializable]
    public class FrameClip : UnityEngine.ISerializationCallbackReceiver
    {
        public ulong ID;
        public int StartFrame;
        public int Length = 1;
        public string Name;
        public string Comment;

        public int GroupId => (int)ID >> 32;

        [UnityEngine.SerializeField]
        private SerializationData serializeData;
        [NonSerialized]
        private IFrameLineClipData data;
        public IFrameLineClipData Data
        {
            get
            {
                if (data == null)
                {
                    data = SerializerHelper.Deserialize(serializeData) as IFrameLineClipData;
                }
                return data;
            }
        }

        public string TypeGUID => serializeData.TypeGUID;

        public void SetData(IFrameLineClipData clipData)
        {
            data = clipData;
            OnBeforeSerialize();
        }

        public void OnAfterDeserialize()
        {
            data = null;
        }

        public void OnBeforeSerialize()
        {
            if (data != null)
                serializeData = SerializerHelper.Serialize(data);
        }

        public static implicit operator FrameClipRef(FrameClip exists)
        {
            if (exists == null)
                return FrameClipRef.Empty;
            return new FrameClipRef() {ID = exists.ID,  Clip = exists};
        }
    }

}
