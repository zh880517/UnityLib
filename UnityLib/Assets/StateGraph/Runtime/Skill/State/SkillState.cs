
public abstract class SkillState : SkillNode
{
    /// <summary>
    /// 开始执行节点时调用
    /// </summary>
    /// <param name="context"></param>
    /// <returns>用来缓存State运行状态, 可为null</returns>
    public virtual SkillStateData OnEnter(SkillContext context) { return null; }

    /// <summary>
    /// 每一个逻辑帧执行一次，调用OnEnter 后当前帧就开始执行
    /// </summary>
    /// <param name="context"></param>
    /// <param name="times">已经执行的次数</param>
    /// <returns>是否继续执行，返回false当前帧调用OnExit</returns>
    public abstract bool Execute(SkillContext context, SkillStateData data);
    public virtual void OnExit(SkillContext context, SkillStateData data) { }
}

public abstract class SkillStateT<T> : SkillState where T : SkillStateData
{
    public override SkillStateData OnEnter(SkillContext context) 
    { 
        return DoEnter(context); 
    }
    public override bool Execute(SkillContext context, SkillStateData data)
    {
        return DoExecute(context, data as T);
    }
    public override void OnExit(SkillContext context, SkillStateData data) 
    {
        DoExit(context, data as T);
    }

    protected abstract T DoEnter(SkillContext context);

    protected abstract bool DoExecute(SkillContext context, T data);
    protected virtual void DoExit(SkillContext context, T data) { }
}
