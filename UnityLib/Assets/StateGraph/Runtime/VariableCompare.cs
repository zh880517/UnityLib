using System;

public enum NumberCompareType
{
    Less = 0,
    LessEquel = 1,
    Equel = 2,
    GreaterEquel = 3,
    Greater = 4,
}
[Serializable]
public struct VariableCompare
{
    [DisaplayName("左")]
    public ReadVariable Left;
    [DisaplayName("比较类型")]
    public NumberCompareType CompareType;
    [DisaplayName("右")]
    public ReadVariable Right;

    public bool Compare(RuntimeBlackboard blackboard)
    {
        float left = Left.GetValue(blackboard);
        float right = Right.GetValue(blackboard);
        switch (CompareType)
        {
            case NumberCompareType.Less:
                return left < right;
            case NumberCompareType.LessEquel:
                return left <= right;
            case NumberCompareType.Equel:
                return left == right;
            case NumberCompareType.GreaterEquel:
                return left >= right;
            case NumberCompareType.Greater:
                return left > right;
        }
        return true;
    }
}
