using System.Collections.Generic;
namespace LiteECS
{
    public class Context
    {
        private int idIndex = 1;
        protected readonly Dictionary<int, Entity> entitis = new Dictionary<int, Entity>();
        protected readonly List<IComponentCollector> collectors;
        public int Generation { get; private set; }

        public Context(int componentTypeCount)
        {
            collectors = new List<IComponentCollector>(componentTypeCount);
        }

        public void InitComponentCollector<T>() where T : class, IComponent, new()
        {
            if (ComponentIdentity<T>.Id == -1)
            {
                throw new System.Exception(string.Format("Component类型未初始化 => {0} ", typeof(T).FullName));
            }
            if (collectors[ComponentIdentity<T>.Id] != null)
            {
                throw new System.Exception(string.Format("ComponentId 重复或重复注册 => {0}", typeof(T).FullName));
            }
            collectors[ComponentIdentity<T>.Id] = new ComponentCollector<T>();
        }

        public void InitUniqueComponentCollector<T>() where T : class, IComponent, IUnique, new()
        {
            if (ComponentIdentity<T>.Id == -1)
            {
                throw new System.Exception(string.Format("Component类型未初始化 => {0} ", typeof(T).FullName));
            }
            if (collectors[ComponentIdentity<T>.Id] != null)
            {
                throw new System.Exception(string.Format("ComponentId 重复或重复注册 => {0}", typeof(T).FullName));
            }
            collectors[ComponentIdentity<T>.Id] = new UniqueComponentCollector<T>();
        }

        protected Entity Find(int id)
        {
            if (entitis.TryGetValue(id, out var enity))
            {
                return enity;
            }
            return null;
        }

        protected int GenId()
        {
            int newId;
            do
            {
                newId = idIndex++;
                if (idIndex == 0)
                {
                    idIndex++;
                    ++Generation;
                }
            } while (entitis.ContainsKey(newId));
            return newId;
        }

        public void DestroyEntity(Entity entity)
        {
            if (entity.IsDestroyed)
                throw new System.Exception("Entity has destroyed");
            for (int i = 0; i < collectors.Count; ++i)
            {
                collectors[i].Remove(entity);
            }
            entitis.Remove(entity.Id);
        }

        public T AddComponent<T>(Entity entity, bool forceModify) where T : class, IComponent, new()
        {
            if (entity.IsDestroyed)
                throw new System.Exception("Entity has destroyed");
            return collectors[ComponentIdentity<T>.Id].Add(entity, forceModify) as T;
        }

        public T ModifyComponent<T>(Entity entity) where T : class, IComponent, new()
        {
            if (entity.IsDestroyed)
                throw new System.Exception("Entity has destroyed");
            return collectors[ComponentIdentity<T>.Id].Modify(entity) as T;
        }

        public T GetComponent<T>(Entity entity) where T : class, IComponent, new()
        {
            if (entity.IsDestroyed)
                throw new System.Exception("Entity has destroyed");
            return collectors[ComponentIdentity<T>.Id].Get(entity) as T;
        }

        public void RemoveComponent<T>(Entity entity) where T : class, IComponent, new()
        {
            if (entity.IsDestroyed)
                throw new System.Exception("Entity has destroyed");
            collectors[ComponentIdentity<T>.Id].Remove(entity);
        }

        public void RemoveAll<T>() where T : class, IComponent, new()
        {
            collectors[ComponentIdentity<T>.Id].RemoveAll();
        }

    }
}