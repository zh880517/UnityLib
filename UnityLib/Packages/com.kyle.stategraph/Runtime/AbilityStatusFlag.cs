using UnityEngine;

public class AbilityStatusFlag : ScriptableObject
{
    const string Path = "Assets/Config/AbilityStatusFlag.asset";
    private static AbilityStatusFlag _instance;
    public static AbilityStatusFlag Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<AbilityStatusFlag>(Path);
            }
            return _instance;
        }
    }
    [SerializeField]
    private string[] flags;
    public string[] Flags
    {
        get
        {
            if (flags == null || flags.Length == 0)
            {
                flags = new string[32];
                for (int i =0; i<31; ++i)
                {
                    flags[i] = $"StatusFlag {i+1}";
                }
                flags[31] = "None Flag";
            }
            return flags;
        }
    }

}
