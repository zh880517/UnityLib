namespace LiteECS
{
    public interface IComponentCollector
    {
        int Count { get; }
        IComponent Add(Entity entity, bool forceModify);
        IComponent Get(Entity entity);
        void Remove(Entity entity);
        void RemoveAll();
        IComponent Modify(Entity entity);
        void RegistEventGroup(IEventGroup eventGroup);
        void RemoveEventGroup(IEventGroup eventGroup);
    }

    public interface IComponentCollectorT<T> : IComponentCollector where T : class, IComponent, new()
    {
        Entity Find(ref int startIndex, System.Func<T, bool> condition = null);
        Entity Find(ref int startIndex, out T component, System.Func<T,bool> condition = null);
    }
}
