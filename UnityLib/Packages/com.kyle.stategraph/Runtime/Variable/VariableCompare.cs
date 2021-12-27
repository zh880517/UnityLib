using System;

public enum NumberCompareType
{
    [DisplayName("<")]
    Less = 0,
    [DisplayName("<=")]
    LessEqual = 1,
    [DisplayName("==")]
    Equal = 2,
    [DisplayName(">=")]
    GreaterEqual = 3,
    [DisplayName(">")]
    Greater = 4,
    [DisplayName("概率0-1")]
    Probability = 5,
    [DisplayName("概率0-100")]
    ProbabilityPercent = 6,
}
[Serializable]
public struct VariableCompare
{
    [DisplayName("左")]
    public ReadVariable Left;
    [DisplayName("比较类型")]
    public NumberCompareType CompareType;
    [DisplayName("右")]
    public ReadVariable Right;
}

[Serializable]
public struct LevelVariableCompare
{
    [DisplayName("左")]
    public ReadLevelVariable Left;
    [DisplayName("比较类型")]
    public NumberCompareType CompareType;
    [DisplayName("右")]
    public ReadLevelVariable Right;
}