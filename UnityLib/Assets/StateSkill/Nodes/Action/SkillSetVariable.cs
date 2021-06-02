
[TypeIdentify("077f471e-1919-46b6-90a2-71af4e2ced4e")]
[DisaplayName("设置变量")]
public class SkillSetVariable : SkillAction
{
    public WriteVariable Key;
    public ReadVariable Value;
    public override void Execute(SkillContext context)
    {
        Key.SetValue(context.Blackboard, Value.GetValue(context.Blackboard));
    }
}