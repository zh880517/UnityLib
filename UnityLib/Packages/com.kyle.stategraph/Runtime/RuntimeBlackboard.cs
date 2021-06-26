using System.Collections.Generic;

public class RuntimeBlackboard
{
    private Dictionary<string, float> Variables = new Dictionary<string, float>();
    public StateRuntimeObserve Observe;
    public float GetValue(string key)
    {
        if (Variables.TryGetValue(key, out float val))
        {
            return val;
        }
        return 0;
    }

    public void SetValue(string key, float val)
    {
        Variables[key] = val;
        Observe?.OnVariableChange(key, val);
    }

    public void Clear()
    {
        Variables.Clear();
    }
}
