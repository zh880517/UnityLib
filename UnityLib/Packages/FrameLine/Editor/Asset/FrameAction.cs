using System;

namespace FrameLine
{
    [Serializable]
    public class FrameAction : UnityEngine.ISerializationCallbackReceiver
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
        private IFrameActionData data;
        public IFrameActionData Data
        {
            get
            {
                if (data == null)
                {
                    data = SerializerHelper.Deserialize(serializeData) as IFrameActionData;
                }
                return data;
            }
        }

        public string TypeGUID => serializeData.TypeGUID;

        public void SetData(IFrameActionData clipData)
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

        public static implicit operator FrameActionRef(FrameAction exists)
        {
            if (exists == null)
                return FrameActionRef.Empty;
            return new FrameActionRef() {ID = exists.ID,  Action = exists};
        }
    }

}
