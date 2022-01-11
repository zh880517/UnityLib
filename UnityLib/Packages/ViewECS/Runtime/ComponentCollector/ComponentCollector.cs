using System.Collections.Generic;
namespace ViewECS
{
    public class ComponentCollector<T> : ITComponentCollector<T> where T : IViewComponent, new()
    {
        private readonly Dictionary<EntityIdentity, T> components = new Dictionary<EntityIdentity, T>();
        private readonly Queue<T> pool = new Queue<T>();
        public T Add(EntityIdentity id)
        {
            if (!components.TryGetValue(id, out T component))
            {
                component = Create();
                components.Add(id, component);
            }
            return component;
        }

        public T Get(EntityIdentity id)
        {
            return components[id];
        }

        public void Remove(EntityIdentity id)
        {
            if (components.TryGetValue(id, out T component))
            {
                pool.Enqueue(component);
                ComponentClear.Clear(component);
                components.Remove(id);
            }
        }

        private T Create()
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            return new T();
        }
    }

}
