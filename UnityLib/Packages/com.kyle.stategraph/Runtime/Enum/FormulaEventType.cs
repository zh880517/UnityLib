public enum FormulaEventType
{
    [DisplayName("暴击")]
    Critical = 1,//暴击
    [DisplayName("格挡")]
    Block = 2,//格挡
    [DisplayName("免疫")]
    Immune = 4,//免疫
    [DisplayName("抵消")]
    Counteract = 8,//抵消
    [DisplayName("闪避")]
    Dodge = 16,//闪避
    [DisplayName("伤害上限")]
    DamageLimit = 32,//伤害上限
    [DisplayName("免疫死亡")]
    ImmunityDead = 64,//免疫死亡
}
