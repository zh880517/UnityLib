//一元运算类型
public enum OneVariableOperationType
{
    [DisplayName("y=x")]
    Immediate = 0,
    [DisplayName("y=(1+x)*y")]
    PercentIncrease = 1,
    [DisplayName("y=(1-x)*y")]
    PercentDecrease = 2,
}
