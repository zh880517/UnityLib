namespace ViewECS
{
    public class TEntity<TComponent> : Entity where TComponent : IViewComponent
    {
        public TEntity(IContext context, int index): base(context, index)
        {

        }
        public T Add<T>() where T : TComponent
        {
            return owner.AddComponent<T>(Id);
        }

        public T Get<T>() where T : TComponent
        {
            return owner.GetComponent<T>(Id);
        }

        public T Modify<T>() where T : TComponent
        {
            return owner.ModifyComponent<T>(Id);
        }

        public bool Has<T>() where T : TComponent
        {
            return owner.GetComponent<T>(Id) != null;
        }

        public void Remove<T>() where T : TComponent
        {
            owner.RemoveComponent<T>(Id);
        }

    }
}