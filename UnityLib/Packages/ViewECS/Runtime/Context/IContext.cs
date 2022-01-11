namespace ViewECS
{
    public interface IContext
    {
        T GetEntity<T>(EntityIdentity id) where T : Entity;

        T AddComponent<T>(EntityIdentity id) where T : IViewComponent;

        T GetComponent<T>(EntityIdentity id) where T : IViewComponent;

        T ModifyComponent<T>(EntityIdentity id) where T : IViewComponent;

        void RemoveComponent<T>(EntityIdentity id) where T : IViewComponent;

    }
}