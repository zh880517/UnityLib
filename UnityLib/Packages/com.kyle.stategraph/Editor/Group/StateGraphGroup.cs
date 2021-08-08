using System.Collections.Generic;
using UnityEngine;

public class StateGraphGroup : ScriptableObject
{
    public List<string> Groups = new List<string>();

    private string[] _names;
    public string[] Names 
    {
        get
        {
            if (_names == null || _names.Length != Groups.Count)
            {
                _names = Groups.ToArray();
            }
            return _names;
        }
    }

    public void UpdateNames()
    {
        _names = null;
    }

    public string GetName(int idx)
    {
        if (idx >= 0 && idx < Groups.Count)
            return Groups[idx];
        return "";
    }
}
