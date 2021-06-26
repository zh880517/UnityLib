using System;

[Serializable]
public struct ReadVariable
{
    public float Value;
    public string Key;
    public bool Share;

    public float GetValue(RuntimeBlackboard blackboard)
    {
        if (!Share)
        {
            return Value;
        }
        return blackboard.GetValue(Key);
    }
}
