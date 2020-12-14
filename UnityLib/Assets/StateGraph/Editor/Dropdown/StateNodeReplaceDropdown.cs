using System;

public class StateNodeReplaceDropdown : StateNodeTypeDropDown
{
    private StateNodeRef node;

    public StateNodeReplaceDropdown(StateGraphView view, StateNodeRef replaceNode) :base(view)
    {
        node = replaceNode;
    }

    protected override bool CheckType(Type type)
    {
        return View.Graph.CheckReplace(node.Node.NodeData.GetType(), type);
    }

    protected override void OnSelectType(Type type)
    {
        View.RepleaceNode(node.Node, type);
    }
}
