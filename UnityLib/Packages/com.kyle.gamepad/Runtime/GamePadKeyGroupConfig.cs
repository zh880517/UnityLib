using UnityEngine;

public class GamePadKeyGroupConfig : MonoBehaviour
{
    [System.Serializable]
    public struct GroupConfig
    {
        public int Key;
        public GamePadManager.KeyGroupType Group;
    }

    public GroupConfig[] Configs;

    void Start()
    {
        var mgr = GamePadManager.Instance;
        foreach (var config in Configs)
        {
            mgr.SetKeyGroup(config.Key, config.Group);
        }
    }

    
}
