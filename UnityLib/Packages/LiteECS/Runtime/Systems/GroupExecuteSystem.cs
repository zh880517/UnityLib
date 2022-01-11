namespace LiteECS
{
    public abstract class GroupExecuteSystem<TEntity, TComponent> : IExecuteSystem where TComponent : class, IComponent, new() where TEntity : Entity
    {
        protected ContextT<TEntity> context;
        public GroupExecuteSystem(ContextT<TEntity> context)
        {
            this.context = context;
        }

        public void OnExecute()
        {
            var group = context.CreatGroup<TComponent>();
            while(group.MoveNext())
            {
                OnExecuteEntity(group.Entity, group.Component);
            }
        }

        protected abstract void OnExecuteEntity(TEntity entity, TComponent component);
    }

}