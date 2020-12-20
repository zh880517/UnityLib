public abstract class SkillState<T> : SkillNode where T : SkillStateData
{
    /// <summary>
    /// 开始执行节点时调用
    /// </summary>
    /// <param name="context"></param>
    /// <returns>用来缓存State运行状态, 可为null</returns>
    public virtual T OnEnter(SkillContext context) { return null; }
    /// <summary>
    /// 每一个逻辑帧执行一次，调用OnEnter 后当前帧就开始执行
    /// </summary>
    /// <param name="context"></param>
    /// <param name="times">已经执行的次数</param>
    /// <returns>是否继续执行，返回false当前帧调用OnExit</returns>
    public abstract bool Execute(SkillContext context, T data);
    public virtual void OnExit(SkillContext context, T data) { }
}
