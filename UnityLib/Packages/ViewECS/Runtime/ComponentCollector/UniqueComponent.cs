namespace ViewECS
{
    public class UniqueComponent<T> : ITComponentCollector<T> where T : IViewComponent, new()
    {
        private readonly T component = new T();
        private EntityIdentity owner = EntityIdentity.None;
        private readonly IContext context;
        public UniqueComponent(IContext context)
        {
            this.context = context;
        }
        public T Add(EntityIdentity id)
        {
            if (id != owner)
            {
                if (owner != EntityIdentity.None)
                {
                    context.ModifyComponent<T>(id);
                }
                owner = id;
                ComponentClear.Clear(component);
            }
            return component;
        }

        public T Get(EntityIdentity id)
        {
            if (id == owner)
                return component;
            return default;
        }

        public void Remove(EntityIdentity id)
        {
            if (id == owner)
            {
                owner = EntityIdentity.None;
                ComponentClear.Clear(component);
            }
        }
    }

}
