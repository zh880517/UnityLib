using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class BlackboardVariable
{
    public string Name = string.Empty;
    public double DefultValue;
    public string Commit;
    public bool ReadOnly;
}

[Serializable]
public class StateBlackboard
{
    public List<BlackboardVariable> Variables = new List<BlackboardVariable>();

    private string[] _names;
    public string[] Names
    {
        get
        {
            if (_names == null || Variables.Count != _names.Length)
            {
                _names = Variables.Select(it => it.Name).ToArray();
            }
            return _names;
        }
    }

    public bool HasName(string name)
    {
        return Variables.Exists(it => it.Name == name);
    }

    public int NameIndex(string name)
    {
        return Variables.FindIndex(it => it.Name == name);
    }

    public void RemoveAt(int idx)
    {
        Variables.RemoveAt(idx);
        _names = null;
    }

    public void Add(string name)
    {
        if (!HasName(name))
        {
            Variables.Add(new BlackboardVariable { Name = name, DefultValue = 0 });
            _names = null;
        }
    }
}
