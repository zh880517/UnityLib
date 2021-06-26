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

    public bool Compare(RuntimeBlackboard blackboard)
    {
        float left = Left.GetValue(blackboard);
        float right = Right.GetValue(blackboard);
        switch (CompareType)
        {
            case NumberCompareType.Less:
                return left < right;
            case NumberCompareType.LessEqual:
                return left <= right;
            case NumberCompareType.Equal:
                return left == right;
            case NumberCompareType.GreaterEqual:
                return left >= right;
            case NumberCompareType.Greater:
                return left > right;
        }
        return true;
    }
}
