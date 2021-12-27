using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "技能特性", fileName = "AbilityAttribute")]
public class AbilityAttribute : ScriptableObject
{
    [System.Serializable]
    public class AttributeInfo
    {
        public string Name;
        public string Desc;
        public int Group;
    }

    public List<AttributeInfo> Attributes = new List<AttributeInfo>();
    public IEnumerable<string> Names 
    {
        get
        {
            return Attributes.Where(it=>!string.IsNullOrWhiteSpace(it.Name)).Select(it => it.Name);
        }
    }
    [SerializeField]
    private string[] groupName;
    public string[] GroupName
    {
        get
        {
            if (groupName == null)
            {
                groupName = new string[32];
                for (int i=0; i<32; ++i)
                {
                    groupName[i] = $"Group {i+1}";
                }
            }
            return groupName;
        }
    }
    
    const string Path = "Assets/Config/AbilityAttribute.asset";
    private static AbilityAttribute _instance;
    public static AbilityAttribute Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<AbilityAttribute>(Path);
            }
            return _instance;
        }
    }
}
