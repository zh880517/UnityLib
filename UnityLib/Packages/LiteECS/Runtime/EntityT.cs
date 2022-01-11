namespace LiteECS
{
    public class EntityT<TComponent> : Entity where TComponent : IComponent
    {
        public EntityT(Context context, int id) : base(context, id)
        {
        }

        public T Get<T>() where T : class, TComponent, new()
        {
            return owner.GetComponent<T>(this);
        }

        public bool Has<T>() where T : class, TComponent, new()
        {
            return owner.GetComponent<T>(this) != null;
        }
        /// <summary>
        /// 添加组件，如果组件存在，则返回组件，否则新建一个
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="forceModify">如果组件存在的话是否触发ComponentEvent.OnModify</param>
        /// <returns>组件对象</returns>
        public T Add<T>(bool forceModify = false) where T : class, TComponent, new()
        {
            return owner.AddComponent<T>(this, forceModify);
        }
        /// <summary>
        /// 获取Component，并且触发ComponentEvent.OnModify
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Modify<T>() where T : class, TComponent, new()
        {
            return owner.ModifyComponent<T>(this);
        }

        public void Remove<T>() where T : class, TComponent, new()
        {
            owner.RemoveComponent<T>(this);
        }
    }

}
