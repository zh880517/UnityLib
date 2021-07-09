using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public abstract class StateNodeTypeDropDown : AdvancedDropdown
{
    protected StateGraphView View;
    protected Dictionary<int, Type> TypeCache = new Dictionary<int, Type>();
    public StateNodeTypeDropDown(StateGraphView view) : base(new AdvancedDropdownState())
    {
        View = view;
        minimumSize = new Vector2(300, 400);
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("选择节点");
        for (int i=0; i<View.VaildTypes.Count; ++i)
        {
            var type = View.VaildTypes[i];
            if (type.GetCustomAttribute<HiddenInTypeCreaterAttribute>() != null || !CheckType(type))
                continue;
            var dpName = type.GetCustomAttribute<DisplayNameAttribute>(false);
            string name = dpName == null ? type.Name : string.Format("{0}({1})", dpName.Name, type.Name);
            var item = new AdvancedDropdownItem(name) { id = i };
            GetRoot(type, root).AddChild(item);
        }
        return root;
    }
    private static readonly List<string> tmpList = new List<string>();

    protected virtual bool CheckType(Type type)
    {
        return true;
    }

    protected AdvancedDropdownItem GetRoot(Type type, AdvancedDropdownItem root)
    {
        tmpList.Clear();
        while (type != null)
        {
            var catalog = type.GetCustomAttribute<CatalogAttribute>(false);
            if (catalog != null && !string.IsNullOrEmpty(catalog.Name))
            {
                tmpList.Add(catalog.Name);
            }
            type = type.BaseType;
        }
        for (int i = tmpList.Count - 1; i >= 0; --i)
        {
            var child = root.children.FirstOrDefault(it => it.name == tmpList[i]);
            if (child == null)
            {
                child = new AdvancedDropdownItem(tmpList[i]);
                root.AddChild(child);
            }
            root = child;
        }
        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        int id = item.id;
        if (id < View.VaildTypes.Count)
        {
            OnSelectType(View.VaildTypes[id]);
        }
    }

    protected abstract void OnSelectType(Type type);
}
