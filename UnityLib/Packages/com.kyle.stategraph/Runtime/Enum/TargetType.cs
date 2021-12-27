public enum TargetType
{
    [DisplayName("友方")]
    Friend = 1,
    [DisplayName("敌方")]
    Enemy = 2,
    [DisplayName("可破坏物")]
    Broken = 4,
    [DisplayName("障碍物")]
    Obstacle = 8,
    [DisplayName("自己")]
    Self = 16,
    All = -1,
}
