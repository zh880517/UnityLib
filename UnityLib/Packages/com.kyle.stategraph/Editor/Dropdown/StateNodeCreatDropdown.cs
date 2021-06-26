using System;
using UnityEngine;

public class StateNodeCreatDropdown : StateNodeTypeDropDown
{
    private Vector2 pos;
    private StateNodeRef node;
    private bool isOut;
    private bool isChild;
    public StateNodeCreatDropdown(StateGraphView view, StateNodeRef node, bool isOut, bool isChild) :base(view)
    {
        this.node = node;
        this.isChild = isChild;
        this.isOut = isChild? true : isOut;
        pos = view.Canvas.MouseInWorld;
    }

    protected override bool CheckType(Type type)
    {
        if (!isChild)
            return true;
        return View.Graph.CheckChildType(node.Node, type);
    }

    protected override void OnSelectType(Type type)
    {
        View.RegistUndo("add node");
        StateNode newNode = View.Graph.AddNode(Activator.CreateInstance(type) as IStateNode, new Rect(pos, StateGraphView.NODE_SIZE));
        View.UpdateBounds(newNode);
        if (node)
        {
            StateNode from;
            StateNode to;
            if (isOut)
            {
                from = node.Node;
                to = newNode;
            }
            else
            {
                from = newNode;
                to = node.Node;
            }
            View.CreateLink(from, to, isChild, false);
        }
    }
}
