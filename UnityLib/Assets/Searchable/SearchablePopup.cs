using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SearchablePopup<TKey> : PopupWindowContent
{
    private const float ROW_HEIGHT = 16.0f;
    private const float ROW_INDENT = 8.0f;

    private List<KeyValuePair<TKey, string>> items;
    private List<KeyValuePair<TKey, string>> filterList;
    private string searchContent;

    public static TKey SelectKey;
    public static int ControllId;
    public static bool IsClosed = true;

    private Vector2 scrollPos;
    private int hoverIndex;
    private int scrollToIndex;
    private float scrollOffset;

    public static void Popup(Rect activatorRect, TKey selectKey, IEnumerable<KeyValuePair<TKey, string>> list, SearchablePopup<TKey> instance)
    {
        instance.items = list.ToList();
        instance.searchContent = "";
        SelectKey = selectKey;
        instance.Filter("");
        IsClosed = false;
        PopupWindow.Show(activatorRect, instance);
    }

    public static TKey GetSelectKey(TKey key, int controllId)
    {
        if (controllId == ControllId && IsClosed)
        {
            if (!key.Equals(SelectKey))
            {
                GUI.changed = true;
                key = SelectKey;
            }
            ControllId = 0;
            SelectKey = default;
        }
        return key;
    }

    public void Filter(string search)
    {
        if (filterList == null)
            filterList = new List<KeyValuePair<TKey, string>>();
        else
            filterList.Clear();
        if (search != null)
            search = search.ToLower();

        for (int i=0; i<items.Count; ++i)
        {
            if (string.IsNullOrEmpty(search) || items[i].Value.ToLower().Contains(search))
            {
                filterList.Add(items[i]);
                if (SelectKey.Equals(items[i].Value))
                {
                    hoverIndex = filterList.Count - 1;
                }
            }
        }
    }
    private void Repaint()
    { 
        if (editorWindow)
        {
            editorWindow.Repaint();
        }
    }
    public override void OnOpen()
    {
        base.OnOpen();
        EditorApplication.update += Repaint;
    }

    public override void OnClose()
    {
        base.OnClose();
        EditorApplication.update -= Repaint;
    }
    public override Vector2 GetWindowSize()
    {
        return new Vector2(base.GetWindowSize().x,
            Mathf.Min(600, filterList.Count * ROW_HEIGHT + 5 +
            EditorStyles.toolbar.fixedHeight));
    }
    public override void OnGUI(Rect rect)
    {
        Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
        Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax + 5, rect.xMax, rect.yMax);
        HandleKeyboard();
        DrawSearch(searchRect);
        DrawSelectionArea(scrollRect);
    }

    private void HandleKeyboard()
    {
        var e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.DownArrow)
            {
                hoverIndex = Mathf.Min(filterList.Count - 1, hoverIndex + 1);
                Event.current.Use();
                scrollToIndex = hoverIndex;
                scrollOffset = ROW_HEIGHT;
            }
            if (e.keyCode == KeyCode.UpArrow)
            {
                hoverIndex = Mathf.Max(0, hoverIndex - 1);
                Event.current.Use();
                scrollToIndex = hoverIndex;
                scrollOffset = -ROW_HEIGHT;
            }
            if (Event.current.keyCode == KeyCode.Return)
            {
                if (hoverIndex >= 0 && hoverIndex < filterList.Count)
                {
                    DoSelect(filterList[hoverIndex].Key);
                }
            }
            if (Event.current.keyCode == KeyCode.Escape)
            {
                EditorWindow.focusedWindow.Close();
            }
        }
    }

    private static GUIStyle SearchBox = "ToolbarSeachTextField";
    private static GUIStyle CancelButton = "ToolbarSeachCancelButton";
    private static GUIStyle DisabledCancelButton = "ToolbarSeachCancelButtonEmpty";
    private static GUIStyle Selection = "SelectionRect";

    private const string SEARCH_CONTROL_NAME = "EnumSearchText";

    private void DrawSearch(Rect rect)
    {
        if (Event.current.type == EventType.Repaint)
            EditorStyles.toolbar.Draw(rect, false, false, false, false);
        Rect searchRect = new Rect(rect);
        searchRect.xMin += 6;
        searchRect.xMax -= 6;
        searchRect.y += 2;
        searchRect.width -= CancelButton.fixedWidth;

        GUI.FocusControl(SEARCH_CONTROL_NAME);
        GUI.SetNextControlName(SEARCH_CONTROL_NAME);
        EditorGUI.BeginChangeCheck();
        searchContent = GUI.TextField(searchRect, searchContent, SearchBox);
        if (EditorGUI.EndChangeCheck())
        {
            Filter(searchContent);
            hoverIndex = 0;
            scrollPos = Vector2.zero;
        }

        searchRect.x = searchRect.xMax;
        searchRect.width = CancelButton.fixedWidth;

        if (string.IsNullOrEmpty(searchContent))
            GUI.Box(searchRect, GUIContent.none, DisabledCancelButton);
        else if (GUI.Button(searchRect, "x", CancelButton))
        {
            searchContent = "";
            Filter(searchContent);
            scrollPos = Vector2.zero;
        }
    }

    private void DrawSelectionArea(Rect scrollRect)
    {
        Rect contentRect = new Rect(0, 0,
                scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth,
                filterList.Count * ROW_HEIGHT);
        using(var scroll = new GUI.ScrollViewScope(scrollRect, scrollPos, contentRect))
        {
            scrollPos = scroll.scrollPosition;
            Rect rowRect = new Rect(0, 0, scrollRect.width, ROW_HEIGHT);
            for (int i=0; i<filterList.Count; ++i)
            {
                if (scrollToIndex == i &&
                    (Event.current.type == EventType.Repaint
                     || Event.current.type == EventType.Layout))
                {
                    Rect r = new Rect(rowRect);
                    r.y += scrollOffset;
                    GUI.ScrollTo(r);
                    scrollToIndex = -1;
                    scrollPos.x = 0;
                }
                if (rowRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseMove ||
                        Event.current.type == EventType.ScrollWheel)
                        hoverIndex = i;
                    if (Event.current.type == EventType.MouseDown)
                    {
                        DoSelect(filterList[i].Key);
                    }
                }
                var item = filterList[i];
                DrawRow(rowRect, item.Value, SelectKey.Equals(item.Key), i == hoverIndex);
                rowRect.y = rowRect.yMax;
            }
        }
    }

    private void DoSelect(TKey key)
    {
        SelectKey = key;
        IsClosed = true;
        EditorWindow.focusedWindow.Close();
    }

    private void DrawRow(Rect rowRect, string content, bool isSelect, bool isHover)
    {
        if (isSelect)
            DrawBox(rowRect, Color.cyan);
        else if (isHover)
            DrawBox(rowRect, Color.white);

        Rect labelRect = new Rect(rowRect);
        labelRect.xMin += ROW_INDENT;

        GUI.Label(labelRect, content);
    }

    private static void DrawBox(Rect rect, Color tint)
    {
        Color c = GUI.color;
        GUI.color = tint;
        GUI.Box(rect, "", Selection);
        GUI.color = c;
    }
}
