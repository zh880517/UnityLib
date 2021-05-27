using System;

[Serializable]
public struct ReadVariable
{
    public float Value;
    public string Key;
    public bool UseBlackBoard;

    public float GetValue(RuntimeBlackboard blackboard)
    {
        if (!UseBlackBoard)
        {
            return Value;
        }
        return blackboard.GetValue(Key);
    }
}
