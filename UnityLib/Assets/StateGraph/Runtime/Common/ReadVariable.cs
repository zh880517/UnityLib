using System;
using UnityEngine;

[Serializable]
public struct ReadVariable
{
    [SerializeField]
    private float fixedValue;
    [SerializeField]
    private string keyName;

    public float GetValue(RuntimeBlackboard blackboard)
    {
        if (string.IsNullOrEmpty(keyName))
        {
            return fixedValue;
        }
        return blackboard.GetValue(keyName);
    }
}
