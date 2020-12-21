using UnityEngine;

public struct WriteVariable
{
    [SerializeField]
    private string keyName;

    [Newtonsoft.Json.JsonIgnore]
    public string Key { get { return keyName; } set { keyName = value; } }

    public void SetValue(RuntimeBlackboard blackboard, float val)
    {
        blackboard.SetValue(keyName, val);
    }
}
