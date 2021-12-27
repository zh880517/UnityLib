public enum TwoVariableOperationType
{
    [DisplayName("y = x + z")]
    Add = 1,
    [DisplayName("y = x - z")]
    Minus = 2,
    [DisplayName("y = x * z")]
    Multiply = 3,
    [DisplayName("y = x ÷ z")]
    Divide = 4,
    [DisplayName("y = (x ÷ z)整数")]
    IntDivide = 5,
    [DisplayName("y = (x ÷ z)余数")]
    Mod = 6,
    [DisplayName("y=(1+x)*z")]
    PercentIncrease = 7,
    [DisplayName("y=(1-x)*z")]
    PercentDecrease = 8,
    [DisplayName("y=[x, z]，Clamp")]
    Clamp = 9
}
