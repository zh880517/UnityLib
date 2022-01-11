using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViewECS
{
    public abstract class TContext<TComponent> : IContext where TComponent : IViewComponent
    {
        protected readonly List<Entity> entities = new List<Entity>();
        protected readonly List<IComponentCollector> componentCollectors = new List<IComponentCollector>();
        private readonly Queue<int> emptyIndexs = new Queue<int>();

        protected abstract Entity NewEntity(int index);

        public Entity CreatEntity()
        {
            Entity entity;
            if (emptyIndexs.Count > 0)
            {
                int index = emptyIndexs.Dequeue();
                entity = entities[index];
            }
            else
            {
                entity = NewEntity(entities.Count);
                entities.Add(entity);
            }
            return entity;
        }

        protected virtual void OnEntityDestroy(EntityIdentity id)
        {
            var entity = GetEntity<Entity>(id);
        }

        public T AddComponent<T>(EntityIdentity id) where T : IViewComponent
        {
            throw new System.NotImplementedException();
        }

        public T GetComponent<T>(EntityIdentity id) where T : IViewComponent
        {
            throw new System.NotImplementedException();
        }

        public T GetEntity<T>(EntityIdentity id) where T : Entity
        {
            return FindEntity(id) as T;
        }

        public T ModifyComponent<T>(EntityIdentity id) where T : IViewComponent
        {
            throw new System.NotImplementedException();
        }

        public void RemoveComponent<T>(EntityIdentity id) where T : IViewComponent
        {
            throw new System.NotImplementedException();
        }

        private Entity FindEntity(EntityIdentity id)
        {
            if (id.Index >= 0)
            {
                var entity = entities[id.Index];
                if (entity.Id.SerialNumber == id.SerialNumber)
                    return entity;
            }
            return null;
        }
    }

}
