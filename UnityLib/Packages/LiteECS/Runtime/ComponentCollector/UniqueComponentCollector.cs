using System.Collections.Generic;

namespace LiteECS
{
    public class UniqueComponentCollector<T> : IComponentCollectorT<T> where T : class, IComponent, IUnique, new()
    {
        public int Count => uniqueEntity != null ? 1 : 0;
        private readonly List<IEventGroup> eventGroups = new List<IEventGroup>();

        private Entity uniqueEntity;
        private T uniqueComponent;
        public IComponent Add(Entity entity, bool forceModify)
        {
            if (uniqueEntity == entity)
            {
                for (int i = 0; i < eventGroups.Count; ++i)
                {
                    eventGroups[i].OnModify<T>(entity.Id);
                }
                return uniqueComponent;
            }
            else
            {
                Remove(entity);
            }
            if (uniqueComponent == null)
            {
                uniqueComponent = new T();
            }
            uniqueEntity = entity;
            for (int i = 0; i < eventGroups.Count; ++i)
            {
                eventGroups[i].OnAdd<T>(entity.Id);
            }
            return uniqueComponent;
        }

        public IComponent Get(Entity entity)
        {
            if (entity == uniqueEntity)
                return uniqueComponent;
            return null;
        }

        public T Get()
        {
            if (uniqueEntity)
            {
                return uniqueComponent;
            }
            return null;
        }

        public ECPair<TEntity, T> GetPair<TEntity>() where TEntity : Entity
        {
            if (uniqueEntity)
            {
                return new ECPair<TEntity, T> { Owner = uniqueEntity as TEntity, Value = uniqueComponent };
            }
            return new ECPair<TEntity, T>();
        }

        public Entity Find(ref int startIndex, out T component, System.Func<T, bool> condition)
        {
            if (startIndex == 0 && uniqueEntity != null && (condition == null || condition(uniqueComponent)))
            {
                startIndex = 1;
                component = uniqueComponent;
                return uniqueEntity;
            }
            component = null;
            startIndex = 1;
            return null;
        }

        public Entity Find(ref int startIndex, System.Func<T, bool> condition)
        {
            if (startIndex == 0 && uniqueEntity != null && (condition == null || condition(uniqueComponent)))
            {
                startIndex = 1;
                return uniqueEntity;
            }
            startIndex = 1;
            return null;
        }

        public IComponent Modify(Entity entity)
        {
            if (uniqueEntity == entity)
            {
                for (int i = 0; i < eventGroups.Count; ++i)
                {
                    eventGroups[i].OnModify<T>(entity.Id);
                }
                return uniqueComponent;
            }
            return null;
        }

        public void RegistEventGroup(IEventGroup eventGroup)
        {
            eventGroups.Add(eventGroup);
        }

        public void RemoveEventGroup(IEventGroup eventGroup)
        {
            eventGroups.Remove(eventGroup);
        }

        public void Remove(Entity entity)
        {
            if (entity == uniqueEntity)
            {
                int id = uniqueEntity.Id;
                if (uniqueEntity is IReset resetComp)
                    resetComp.Reset();
                uniqueEntity = null;
                for (int i = 0; i < eventGroups.Count; ++i)
                {
                    eventGroups[i].OnRemove<T>(id);
                }
            }
        }

        public void RemoveAll()
        {
            if (uniqueEntity != null)
            {
                int id = uniqueEntity.Id;
                if (uniqueEntity is IReset resetComp)
                    resetComp.Reset();
                uniqueEntity = null;

                for (int i = 0; i < eventGroups.Count; ++i)
                {
                    eventGroups[i].OnRemove<T>(id);
                }
            }
        }
    }

}
