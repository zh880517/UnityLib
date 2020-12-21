using System;
using UnityEngine;

[Serializable]
public struct ReadVariable
{
    [SerializeField]
    private float fixedValue;
    [SerializeField]
    private string keyName;
    [Newtonsoft.Json.JsonIgnore]
    public string Key { get { return keyName; } set { keyName = value; } }
    [Newtonsoft.Json.JsonIgnore]
    public float Value { get { return fixedValue; } set { fixedValue = value; } }

    public float GetValue(RuntimeBlackboard blackboard)
    {
        if (string.IsNullOrEmpty(keyName))
        {
            return fixedValue;
        }
        return blackboard.GetValue(keyName);
    }
}
