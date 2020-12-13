using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.IMGUI.Controls;

public abstract class StateNodeTypeDropDown : AdvancedDropdown
{
    protected StateGraphView GraphView;
    protected Dictionary<int, Type> TypeCache = new Dictionary<int, Type>();
    public StateNodeTypeDropDown(StateGraphView view) : base(new AdvancedDropdownState())
    {
        GraphView = view;
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("选择节点");
        for (int i=0; i<GraphView.VaildTypes.Count; ++i)
        {
            var type = GraphView.VaildTypes[i];
            if (!CheckType(type))
                continue;
            var dpName = type.GetCustomAttribute<DisaplayNameAttribute>();
            string name = dpName == null ? type.Name : string.Format("{0}(1)", dpName, type.Name);
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
            var catalog = type.GetCustomAttribute<CatalogAttribute>();
            if (catalog != null && !string.IsNullOrEmpty(catalog.Name))
            {
                tmpList.Add(catalog.Name);
            }
            type = type.BaseType;
        }
        for (int i = tmpList.Count - 1; i >= 0; --i)
        {
            var child = root.children.First(it => it.name == tmpList[i]);
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
        if (id < GraphView.VaildTypes.Count)
        {
            OnSelectType(GraphView.VaildTypes[id]);
        }
    }

    protected abstract void OnSelectType(Type type);
}
