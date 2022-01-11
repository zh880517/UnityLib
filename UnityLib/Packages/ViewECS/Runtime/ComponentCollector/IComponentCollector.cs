namespace ViewECS
{
    public interface IComponentCollector
    {
    }

    public interface ITComponentCollector<T> where T : IViewComponent
    {
        T Add(EntityIdentity id);
        T Get(EntityIdentity id);
        void Remove(EntityIdentity id);
    }
}