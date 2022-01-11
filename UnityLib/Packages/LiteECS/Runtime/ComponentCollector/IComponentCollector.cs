namespace LiteECS
{
    public interface IComponentCollector
    {
        int Count { get; }
        IComponent Add(Entity entity, uint version, bool forceModify);
        IComponent Get(Entity entity);
        void Remove(Entity entity);
        void RemoveAll();
        IComponent Modify(Entity entity, uint version);
    }

    public struct EntityFindResult<TComponent> where TComponent : IComponent
    {
        public int Id;
        public int Index;
        public uint Version;
        public TComponent Component;
    }

    public interface IComponentCollectorT<T> : IComponentCollector where T : class, IComponent, new()
    {
        EntityFindResult<T> Find(int startIndex, uint version, System.Func<T, bool> condition = null);
    }


    public class ComponentEntity<T> where T : class, IComponent, new()
    {
        public T Component = new T();
        public Entity Owner;
        public uint Version;


        public void Reset()
        {
            Owner = null;
            Version = 0;
            if (Component is IReset resetComp)
                resetComp.Reset();
        }
    }
}
