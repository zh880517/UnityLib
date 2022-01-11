using UnityEngine;
namespace ViewECS
{
    public class Entity
    {
        protected IContext owner { get; private set; }
        public EntityIdentity Id { get; private set; }
        public int Version { get; private set; }

        public Entity(IContext context, int index)
        {
            owner = context;
            Id = new EntityIdentity(index, 0);
        }

        public static implicit operator bool(Entity exists)
        {
            return exists != null && exists.owner != null;
        }
    }
}