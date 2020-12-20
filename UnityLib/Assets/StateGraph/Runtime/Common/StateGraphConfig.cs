using System.Collections.Generic;

public class StateGraphConfig
{
    public string Name;
    public List<SkillNode> Nodes = new List<SkillNode>();
    public List<StateNodeReleation> Releations = new List<StateNodeReleation>();
    public StateBlackboard Blackboard = new StateBlackboard();
}
