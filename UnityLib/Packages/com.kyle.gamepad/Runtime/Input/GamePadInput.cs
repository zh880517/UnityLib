using System.Collections.Generic;
using UnityEngine;

public class GamePadInput : MonoBehaviour
{
    private readonly HashSet<int> groupCaches = new HashSet<int>();
    public GamePadInputConfig Config;

    private void OnDisable()
    {
        var mgr = GamePadManager.Instance;
        foreach (var key in Config.InputKeys)
        {
            if (key.UID != 0)
            {
                mgr.Release(key.Key, key.UID);
            }
        }
    }

    private void Update()
    {
        if (!Config)
            return;
        groupCaches.Clear();
        var mgr = GamePadManager.Instance;
        foreach (var key in Config.InputKeys)
        {
            if (key.UID == 0)
            {
                key.UID = mgr.GenUID();
            }
            var data = mgr.GetKeyData(key.Key);
            if (!data.Check(key.UID))
                continue;
            int group = mgr.GetKeyGroup(key.Key);
            //同一个组的已经处理过的不在进行处理
            if (group != 0 && groupCaches.Contains(group))
                continue;
            bool isPress = key.TryGet(out Vector2 val);
            if (!data.IsPress && !isPress)
                continue;
            if (isPress && group > 0)
                groupCaches.Add(group);
            if (data.IsPress)
            {
                if (isPress)
                {
                    float magnitude = val.magnitude;
                    if (magnitude > 1)
                        val = (val/magnitude) * (Mathf.Clamp01(magnitude));
                    mgr.ValueChange(key.Key, key.UID, val);
                }
                else
                {
                    mgr.Release(key.Key, key.UID);
                }
            }
            else
            {
                if (isPress)
                    mgr.Press(key.Key, key.UID);
            }
        }
    }
}
