using System;
using System.Collections.Generic;

[Serializable]
public class VariableCompareGroup
{
    public List<VariableCompare> Compares = new List<VariableCompare>();
    [DisplayName("最小数量")]
    public uint MinCount;
}

[Serializable]
public class LevelVariableCompareGroup
{
    public List<LevelVariableCompare> Compares = new List<LevelVariableCompare>();
    [DisplayName("最小数量")]
    public uint MinCount;
}
