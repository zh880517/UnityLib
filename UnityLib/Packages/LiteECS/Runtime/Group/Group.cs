namespace LiteECS
{
    public class Group<T> where T : class, IComponent, new()
    {
        private readonly IComponentCollectorT<T> collector;
        private int index = 0;
        public int Count => collector.Count;

        public Group(IComponentCollectorT<T> collector)
        {
            this.collector = collector;
        }

        public void Reset()
        {
            index = 0;
        }

        public Entity Next()
        {
            return collector.Find(ref index);
        }

        public bool TryGet(out Entity entity, out T component)
        {
            entity = collector.Find(ref index, out component);
            return entity != null;
        }
    }

}
