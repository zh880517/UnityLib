using System;

namespace FrameLine
{

    [Serializable]
    public struct FrameClipRef : IEquatable<FrameClipRef>
    {
        public ulong ID;
        public FrameClip Clip { get; set; }
        public static FrameClipRef Empty = new FrameClipRef();
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public bool Equals(FrameClipRef other)
        {
            return other.ID == ID;
        }
        public override bool Equals(object obj)
        {
            if (obj is FrameClipRef clipRef)
            {
                return Equals(clipRef);
            }
            return false;
        }
        public static bool operator ==(FrameClipRef lhs, FrameClipRef rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(FrameClipRef lhs, FrameClipRef rhs)
        {
            return !lhs.Equals(rhs);
        }
        public static implicit operator bool(FrameClipRef exists)
        {
            return exists.ID != 0;
        }
    }
}
