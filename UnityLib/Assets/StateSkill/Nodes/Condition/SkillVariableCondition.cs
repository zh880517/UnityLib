using System.Collections.Generic;

[TypeIdentify("249f9163-40b7-4308-86b7-33fbbda1a28b")]
[DisaplayName("变量条件")]
public class SkillVariableCondition : SkillCondition
{
    public List<VariableCompare> Compares = new List<VariableCompare>();
    public uint MinCount;
}