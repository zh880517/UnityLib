[TypeIdentify("40558C2E-B6C2-476F-A639-8E8C213ECE35")]
public class SkillActionGroup : SkillNode
{
    public VariableCondition Condition;

    public virtual bool Check(SkillContext context)
    {
        if (Condition == null)
            return true;
        return Condition.Check(context.Blackboard);
    }
}
