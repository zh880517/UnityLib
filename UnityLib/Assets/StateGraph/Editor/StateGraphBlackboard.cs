using System;
using System.Collections.Generic;
[Serializable]
public class StateGraphBlackboard
{
    [Serializable]
    public class Variable
    {
        public string Name = string.Empty;
        public float DefultValue;
    }

    public List<Variable> Variables = new List<Variable>();

    public bool HasName(string name)
    {
        return Variables.Exists(it => it.Name == name);
    }
}
