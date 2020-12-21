using System;
using UnityEngine;

public class StateSkillGraph : StateGraph
{
    public override bool CheckChildType(StateNode parent, Type childType)
    {
        if (childType.IsSubclassOf(typeof(SkillAction)) && parent.NodeType.IsSubclassOf(typeof(SkillActionGroup)))
            return true;
        if (childType.IsSubclassOf(typeof(SkillCondition)) && parent.NodeType.IsSubclassOf(typeof(SkillBranch)))
            return true;
        return false;
    }

    public override bool CheckReplace(Type src, Type dst)
    {

        if (src.IsSubclassOf(typeof(SkillAction)) && dst.IsSubclassOf(typeof(SkillAction)))
            return true;
        if (src.IsSubclassOf(typeof(SkillState)) && dst.IsSubclassOf(typeof(SkillState)))
            return true;

        return false;
    }

    public override bool CheckTypeValid(Type type)
    {
        return type.IsSubclassOf(typeof(SkillNode));
    }

    public override bool IsStack(StateNode node)
    {
        return node.NodeData is SkillBranch || node.NodeData is SkillActionGroup;
    }
    public override bool ChechInput(StateNodeRef node)
    {
        return !(node.Node.NodeData is SkillEntry);
    }

    public override bool CheckOutput(StateNodeRef node)
    {
        return !(node.Node.NodeData is SkillAction);
    }
    public override bool CheckCopy(StateNodeRef node)
    {
        return !(node.Node.NodeData is SkillEntry);
    }

    public override bool CheckDelete(StateNodeRef node)
    {
        return !(node.Node.NodeData is SkillEntry);
    }

    protected override void OnCreat()
    {
        SkillEntry entry = new SkillEntry();
        var node = AddNode(entry, new Rect(Vector2.zero, new Vector2(80, 80)));
        node.Name = "Entry";

    }
}
