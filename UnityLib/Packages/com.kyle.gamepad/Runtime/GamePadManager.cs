using UnityEngine;
using System.Collections.Generic;
using GamePadView;

public class GamePadManager
{

    /// <summary>
    /// 为了方便处理，不分组的Key所属组为0
    /// </summary>
    public enum KeyGroupType
    {
        None = 0,
        Group1 = 1,
        Group2,
        Group3,
        Group4,
        Group5,
        Group6,
        Group7,
        Group8,
        Group9,
    }
    public class KeyData
    {
        public int SourceUID { get; set; }
        public Vector2 Value { get; set; }
        public bool ButtonOnly { get; set; }
        public bool Distable { get; set; }

        public bool Check(int uid)
        {
            return !Distable && (SourceUID == 0 || SourceUID == uid);
        }

        public bool IsPress => SourceUID != 0;
    }

    #region Singleton
    private static GamePadManager _instance;
    public static GamePadManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GamePadManager();
            }
            return _instance;
        }
    }

    public static void Destroy()
    {
        _instance = null;
    }

    #endregion

    private readonly Dictionary<int, BaseView> views = new Dictionary<int, BaseView>();
    private readonly Dictionary<int, KeyData> keyDatas = new Dictionary<int, KeyData>();
    private readonly Dictionary<int, int> keyGroups = new Dictionary<int, int>();
    public bool Disable { get; set; }

    public System.Action<int, bool> OnButtonEvent;
    public System.Action<int, Vector2> OnMoveEvent;

    private int uidIdx;

    public int RegisterView(int key, BaseView unit)
    {
        if (views.ContainsKey(key))
            return 0;
        views[key] = unit;
        var data = GetKeyData(key);
        unit.SetState(data.ButtonOnly);
        return GenUID();
    }

    public int GenUID()
    {
        return ++uidIdx;
    }

    public void SetKeyState(int key, bool isButton)
    {
        var data = GetKeyData(key);
        data.ButtonOnly = isButton;
        if (views.TryGetValue(key, out var view))
        {
            view.SetState(isButton);
        }
    }

    public void UnRegisterView(BaseView unit)
    {
        foreach (var kv in views)
        {
            if (kv.Value == unit)
            {
                views.Remove(kv.Key);
                return;
            }
        }
    }

    public void SetKeyGroup(int key, KeyGroupType group)
    {
        if (group == KeyGroupType.None)
        {
            keyGroups.Remove(key);
            return;
        }
        keyGroups[key] = (int)group;
    }

    public void SetActive(int key, bool active)
    {
        var data = GetKeyData(key);
        if (data.Distable == active)
        {
            if (data.IsPress)
            {
                Release(key, data.SourceUID);
            }
            data.Distable = !active;
        }
    }

    public bool CheckStartInput(int key, int uid)
    {
        if (keyDatas.TryGetValue(key, out var data))
        {
            return data.Check(uid);
        }
        return false;
    }


    public KeyData GetKeyData(int key)
    {
        if (!keyDatas.TryGetValue(key, out var data))
        {
            data = new KeyData();
            keyDatas.Add(key, data);
        }
        return data;
    }

    public int GetKeyGroup(int key)
    {
        keyGroups.TryGetValue(key, out int group);
        return group;
    }

    public void Press(int key, int uid)
    {
        if (Disable)
            return;
        var data = GetKeyData(key);
        if (!data.Check(uid))
            return;
        int group = GetKeyGroup(key);
        if (group != 0)
        {
            foreach (var kv in keyGroups)
            {
                if (kv.Value == group)
                {
                    if (keyDatas.TryGetValue(kv.Key, out var info) && info.IsPress)
                    {
                        Release(kv.Key, info.SourceUID);
                        break;
                    }
                }
            }
        }
        data.SourceUID = uid;
        data.Value = Vector2.zero;
        OnButtonEvent?.Invoke(key, true);
        ViewStateChange(key, true);
    }

    public void Release(int key, int uid)
    {
        if (keyDatas.TryGetValue(key, out var data) && data.SourceUID == uid)
        {
            data.SourceUID = 0;
            OnButtonEvent?.Invoke(key, false);
            ViewStateChange(key, false);
        }
    }

    public void ValueChange(int key, int uid, Vector2 val)
    {
        if (Disable)
            return;
        if (!keyDatas.TryGetValue(key, out var data) || data.ButtonOnly || data.SourceUID != uid || data.Value == val)
            return;
        data.Value = val;
        OnMoveEvent?.Invoke(key, val);
        ViewValueChange(key, val);
    }

    public void ViewStateChange(int key, bool press)
    {
        if (views.TryGetValue(key, out var view))
        {
            if (press)
                view.OnPress();
            else
                view.OnRelease();
        }
    }

    public void ViewValueChange(int key, Vector2 val)
    {
        if (views.TryGetValue(key, out var view))
        {
            view.OnValueChange(val);
        }
    }
}
