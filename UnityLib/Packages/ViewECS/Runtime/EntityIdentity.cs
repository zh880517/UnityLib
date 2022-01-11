namespace ViewECS
{
    public struct EntityIdentity : System.IEquatable<EntityIdentity>
    {
        public int SerialNumber { get; private set; }
        public int Index {get; private set; }

        public long ToUID()
        {
            long uid = SerialNumber;
            uid = uid << 32 | (uint)Index;
            return uid;
        }

        public bool Equals(EntityIdentity other)
        {
            return other.SerialNumber == SerialNumber && other.Index == Index;
        }

        public EntityIdentity(int index, int serial)
        {
            Index = index;
            SerialNumber = serial;
        }

        public EntityIdentity(long uid)
        {
            Index = (int)(uid & 0xFFFFFFFF);
            SerialNumber = (int)(uid >> 32);
        }

        public override bool Equals(object obj)
        {
            return Equals((EntityIdentity)obj);
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode();
        }
        public static bool operator ==(EntityIdentity lhs, EntityIdentity rhs)
        {
            return lhs.SerialNumber == rhs.SerialNumber && lhs.Index == rhs.Index;
        }

        public static bool operator !=(EntityIdentity lhs, EntityIdentity rhs)
        {
            return !(lhs == rhs);
        }

        public static EntityIdentity None => new EntityIdentity(-1, 0);
    }
}
