namespace LiteECS
{
    public abstract class ReactiveExecuteSystem<TEntity, TComponent> : IExecuteSystem where TComponent : class, IComponent, new() where TEntity : Entity
    {
        private readonly int groupId;
        protected ContextT<TEntity> context;
        public ReactiveExecuteSystem(ContextT<TEntity> context)
        {
            groupId = context.RegisterReactiveGroup<TComponent>();
            this.context = context;
        }

        public void OnExecute()
        {
            var group = context.GetReactiveGroup<TComponent>(groupId);
            while (group.MoveNext())
            {
                OnExecuteEntity(group.Entity, group.Component);
            }
        }

        protected abstract void OnExecuteEntity(TEntity entity, TComponent component);
    }
}