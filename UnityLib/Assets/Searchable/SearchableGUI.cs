using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public static class SearchableGUI
{
    class EnumValueName<T> where T : System.Enum
    {
        private static List<KeyValuePair<int, string>> keyValues;
        public static IReadOnlyList<KeyValuePair<int, string>> KeyValues
        {
            get
            {
                if (keyValues == null)
                {
                    var names = System.Enum.GetNames(typeof(T));
                    var values = System.Enum.GetValues(typeof(T));
                    keyValues = new List<KeyValuePair<int, string>>(names.Length);
                    for (int i = 0; i < names.Length; ++i)
                    {
                        keyValues.Add(new KeyValuePair<int, string>((int)values.GetValue(i), names[i]));
                    }
                }
                return keyValues;
            }
        }
    }

    private static int s_PopupHash = "SearchablePopup".GetHashCode();
    private static int GetControlID(Rect activatorRect)
    {
        return GUIUtility.GetControlID(s_PopupHash, FocusType.Keyboard, activatorRect);
    }

    public static int PopInt(Rect position, int selectKey, IEnumerable<KeyValuePair<int, string>> list)
    {
        var item = list.FirstOrDefault(it => it.Key == selectKey);
        string showText = string.IsNullOrEmpty(item.Value) ? selectKey.ToString() : item.Value;
        int controllId = GetControlID(position);
        if (GUI.Button(position, showText, EditorStyles.popup))
        {
            SearchablePopup<int>.ControllId = controllId;
            SearchablePopup<int>.Popup(position, selectKey, list, new SearchablePopup<int>());
        }
        return SearchablePopup<int>.GetSelectKey(selectKey, controllId);
    }


    public static string PopString(Rect position, string selectKey, IEnumerable<KeyValuePair<string, string>> list)
    {
        var item = list.FirstOrDefault(it => it.Key == selectKey);
        string showText = string.IsNullOrEmpty(item.Value) ? selectKey.ToString() : item.Value;
        int controllId = GetControlID(position);
        if (GUI.Button(position, showText, EditorStyles.popup))
        {
            SearchablePopup<string>.ControllId = controllId;
            SearchablePopup<string>.Popup(position, selectKey, list, new SearchablePopup<string>());
        }
        return SearchablePopup<string>.GetSelectKey(selectKey, controllId);
    }


    public static T PopEnum<T>(Rect position, T selectKey) where T : System.Enum
    {
        int controllId = GetControlID(position);
        if (GUI.Button(position, selectKey.ToString(), EditorStyles.popup))
        {
            SearchablePopup<int>.ControllId = controllId;
            
            SearchablePopup<int>.Popup(position, System.Convert.ToInt32(selectKey), EnumValueName<T>.KeyValues, new SearchablePopup<int>());
        }
        if (SearchablePopup<int>.ControllId == controllId && SearchablePopup<int>.IsClosed)
        {
            SearchablePopup<int>.ControllId = 0;
            if (SearchablePopup<int>.SelectKey != System.Convert.ToInt32(selectKey))
            {
                GUI.changed = true;
            }
            return (T)System.Enum.ToObject(typeof(T), SearchablePopup<int>.SelectKey);
        }
        return selectKey;
    }

}
