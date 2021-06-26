using System.Collections.Generic;

public class StateGraphConfig
{
    public string Name;
    public List<StateNodeRelation> Relations = new List<StateNodeRelation>();
    public StateBlackboard Blackboard = new StateBlackboard();
}
