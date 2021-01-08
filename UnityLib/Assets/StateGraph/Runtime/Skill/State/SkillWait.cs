public class SkillWaitData : SkillStateData
{
    public int WaitFrame;
}

[TypeIdentify("68428fdc-b1ed-47fc-aed2-90247f247995")]
[DisaplayName("等待")]
public class SkillWait : SkillStateT<SkillWaitData>
{
    public ReadVariable WaitSecond;

    protected override SkillWaitData DoEnter(SkillContext context)
    {
        return new SkillWaitData();
    }

    protected override bool DoExecute(SkillContext context, SkillWaitData data)
    {
        throw new System.NotImplementedException();
    }
}