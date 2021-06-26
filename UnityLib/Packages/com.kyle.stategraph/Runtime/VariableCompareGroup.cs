using System;
using System.Collections.Generic;

[Serializable]
public class VariableCompareGroup
{
    public List<VariableCompare> Compares = new List<VariableCompare>();
    [DisplayName("最小数量")]
    public uint MinCount;

    public bool Check(RuntimeBlackboard blackboard)
    {
        if (Compares.Count == 0)
            return true;
        int count = 0;
        foreach (var compare in Compares)
        {
            if (compare.Compare(blackboard))
            {
                count++;
                if (count == Compares.Count || (MinCount > 0 && count >= MinCount))
                    return true;
            }
        }

        return false;
    }
}
