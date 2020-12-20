using System;
using System.Collections.Generic;
[Serializable]
public class BlackboardVariable
{
    public string Name = string.Empty;
    public float DefultValue;
}

[Serializable]
public class StateBlackboard
{
    public List<BlackboardVariable> Variables = new List<BlackboardVariable>();

    public bool HasName(string name)
    {
        return Variables.Exists(it => it.Name == name);
    }
}
