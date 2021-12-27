public enum AbilityStateBlockType
{
    [DisplayName("等待结束")]
    WaitFinish = 0,
    [DisplayName("立即执行后续节点")]
    Continue = 1,
    [DisplayName("阻塞当前分支")]
    Blcok = 2,
}
