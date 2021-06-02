using System;
using UnityEngine;

public class StateSkillGraph : StateGraph
{
    public const string SavePath = "Assets/Config/Skill/";
    public override bool CheckChildType(StateNode parent, Type childType)
    {
        if (childType.IsSubclassOf(typeof(SkillAction)) && typeof(SkillActionGroup).IsAssignableFrom(parent.NodeType))
            return true;
        if (childType.IsSubclassOf(typeof(SkillCondition)) && typeof(SkillBranch).IsAssignableFrom(parent.NodeType))
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
        return node.Data is SkillBranch || node.Data is SkillActionGroup;
    }
    public override bool ChechInput(StateNodeRef node)
    {
        return !(node.Node.Data is SkillEntry);
    }

    public override bool CheckOutput(StateNodeRef node)
    {
        return !(node.Node.Data is SkillAction);
    }
    public override bool CheckCopy(StateNodeRef node)
    {
        return !(node.Node.Data is SkillEntry);
    }

    public override bool CheckDelete(StateNodeRef node)
    {
        return !(node.Node.Data is SkillEntry);
    }

    protected override void OnCreat()
    {
        SkillEntry entry = new SkillEntry();
        var node = AddNode(entry, new Rect(Vector2.zero, new Vector2(80, 80)));
        node.Name = "Entry";

    }
}
