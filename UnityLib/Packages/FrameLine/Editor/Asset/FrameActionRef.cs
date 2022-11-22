using System;

namespace FrameLine
{

    [Serializable]
    public struct FrameActionRef : IEquatable<FrameActionRef>
    {
        public ulong ID;
        public int GroupId => (int)ID >> 32;
        public FrameAction Action { get; set; }
        public static FrameActionRef Empty = new FrameActionRef();
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public bool Equals(FrameActionRef other)
        {
            return other.ID == ID;
        }
        public override bool Equals(object obj)
        {
            if (obj is FrameActionRef actionRef)
            {
                return Equals(actionRef);
            }
            return false;
        }
        public static bool operator ==(FrameActionRef lhs, FrameActionRef rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(FrameActionRef lhs, FrameActionRef rhs)
        {
            return !lhs.Equals(rhs);
        }
        public static implicit operator bool(FrameActionRef exists)
        {
            return exists.ID != 0;
        }
    }
}
