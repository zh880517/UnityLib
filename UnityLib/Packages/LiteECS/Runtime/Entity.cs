namespace LiteECS
{
    public class Entity
    {
        public int Id { get; private set; }
        protected Context owner { get; private set; }

        public bool IsDestroyed { get; private set; } = false;

        public Entity(Context context, int id)
        {
            Id = id;
            owner = context;
        }
        /// <summary>
        /// 销毁Entity
        /// 最好创建一个组件，标记这个Entity为Destroy状态，在单独的CleanupSystem里面遍历销毁
        /// 防止在处理Entity的时候调用的接口将Entity删除造成逻辑错误
        /// </summary>
        public void Destroy()
        {
            if (IsDestroyed)
                return;
            if (Id == 1)
            {
                //ILLog.LogError("UniqueEntity 不能调用 Destroy()");
                return;
            }
            if (owner != null)
            {
                owner.DestroyEntity(this);
            }
            owner = null;
            IsDestroyed = true;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public bool Check(Context context)
        {
            return owner == context;
        }

        public static implicit operator bool(Entity exists)
        {
            return exists != null && !exists.IsDestroyed && exists.owner != null;
        }

    }
}