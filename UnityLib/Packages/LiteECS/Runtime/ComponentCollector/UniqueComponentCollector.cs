using System;

namespace LiteECS
{
    public class UniqueComponentCollector<T> : IComponentCollectorT<T> where T : class, IComponent, IUnique, new()
    {
        public int Count => Component.Owner != null ? 1 : 0;
        private ComponentEntity<T> Component = new ComponentEntity<T>();
        public IComponent Add(Entity entity, uint version, bool forceModify)
        {
            if (Component.Owner == entity)
            {
                return Component.Component;
            }
            else
            {
                Remove(entity);
            }
            Component.Owner = entity;
            Component.Version = version;
            return Component.Component;
        }

        public IComponent Get(Entity entity)
        {
            if (entity == Component.Owner)
                return Component.Component;
            return null;
        }

        public T Get()
        {
            if (Component.Owner != null)
            {
                return Component.Component;
            }
            return null;
        }

        public ECPair<TEntity, T> GetPair<TEntity>() where TEntity : Entity
        {
            if (Component.Owner != null)
            {
                return new ECPair<TEntity, T> { Owner = Component.Owner as TEntity, Value = Component.Component };
            }
            return new ECPair<TEntity, T>();
        }

        public IComponent Modify(Entity entity, uint version)
        {
            if (Component.Owner == entity)
            {
                Component.Version = version;
                return Component.Component;
            }
            return null;
        }

        public void Remove(Entity entity)
        {
            if (entity == Component.Owner)
            {
                Component.Reset();
            }
        }

        public void RemoveAll()
        {
            if (Component.Owner != null)
            {
                Component.Reset();
            }
        }

        public EntityFindResult<T> Find(int startIndex, uint version, Func<T, bool> condition = null)
        {
            var result = new EntityFindResult<T>();
            if (startIndex == 0 && Component.Owner != null && version < Component.Version && (condition == null || condition(Component.Component)))
            {
                result.Component = Component.Component;
                result.Version = Component.Version;
                result.Index = 1;
                result.Component = Component.Component;
            }
            return result;
        }
    }

}
