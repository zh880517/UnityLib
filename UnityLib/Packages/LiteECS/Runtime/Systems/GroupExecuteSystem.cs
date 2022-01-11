namespace LiteECS
{
    public abstract class GroupExecuteSystem<TEntity, TComponent> : IExecuteSystem where TComponent : class, IComponent, new() where TEntity : Entity
    {
        private readonly Group<TComponent> group;
        public GroupExecuteSystem(TContext<TEntity> context)
        {
            group = context.CreatGroup<TComponent>();
        }

        public void OnExecute()
        {
            if (group.Count > 0)
            {
                while (group.TryGet(out Entity entity, out TComponent component))
                {
                    OnExecuteEntity(entity as TEntity, component);
                }
                group.Reset();
            }
        }

        protected abstract void OnExecuteEntity(TEntity entity, TComponent component);
    }

}