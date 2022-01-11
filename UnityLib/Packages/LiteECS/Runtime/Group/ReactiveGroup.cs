namespace LiteECS
{
    public struct ReactiveGroup<TEntity, TComponent> where TEntity : Entity where TComponent : class, IComponent, new()
    {
        private int Index;
        private int GroupIndex;
        private uint Version;
        private ContextT<TEntity> Context;
        public TEntity Entity { get; private set; }
        public TComponent Component { get; private set; }

        public ReactiveGroup(int groupIndex, uint version, ContextT<TEntity> context)
        {
            Version = version;
            Context = context;
            Index = 0;
            GroupIndex = groupIndex;
            Entity = null;
            Component = default;
        }

        public bool MoveNext()
        {
            var result = Context.Find<TComponent>(Index, Version, null, GroupIndex);
            Component = result.Component;
            if (result.Id == 0)
            {
                Entity = null;
                return false;
            }
            Entity = Context.FindEntity(result.Id);
            Index = result.Index;
            return true;
        }
    }
}