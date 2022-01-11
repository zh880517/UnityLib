namespace LiteECS
{
    public enum CleanupMode
    {
        DestroyEntity = 0,
        RemoveComponent = 1,
    }
    /// <summary>
    /// 会自动生成System，进行删除或者销毁操作
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class CleanupAttribute : ComponentAttribute
    {
        public CleanupMode Mode { get; private set; }
        public CleanupAttribute(CleanupMode mode)
        {
            Mode = mode;
        }
    }

}
