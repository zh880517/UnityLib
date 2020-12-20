using UnityEngine;

public struct WriteVariable
{
    [SerializeField]
    private string keyName;

    public void SetValue(RuntimeBlackboard blackboard, float val)
    {
        blackboard.SetValue(keyName, val);
    }
}
