public struct WriteVariable
{
    public string Key;

    public void SetValue(RuntimeBlackboard blackboard, float val)
    {
        blackboard.SetValue(Key, val);
    }
}
