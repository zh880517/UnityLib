
using System.Collections.Generic;

[TypeIdentify("249f9163-40b7-4308-86b7-33fbbda1a28b")]
public class SkillVariableCondition : SkillCondition
{
    public List<VariableCompare> Compares = new List<VariableCompare>();
    public uint MinCount;

    public override bool Check(SkillContext context)
    {
        if (Compares.Count == 0)
            return true;
        int count = 0;
        foreach (var compare in Compares)
        {
            if (compare.Compare(context.Blackboard))
            {
                count++;
                if (count == Compares.Count || (MinCount > 0 && count >= MinCount))
                    return true;
            }
        }

        return false;
    }
}