public enum DamageType 
{
    物理 = 0,
    火焰,
    冰霜,
    雷电,
    毒,
    暗影,
    热能,
    直接,
}

public enum DamageTypeMask
{
    物理 = 1,
    火焰 = 2,
    冰霜 = 4,
    雷电 = 8,
    毒 = 16,
    暗影 = 32,
    热能 = 64,
    直接 = 128,
    所有 = -1,
}
