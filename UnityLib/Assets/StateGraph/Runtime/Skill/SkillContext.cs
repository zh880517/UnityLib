public class SkillContext
{
    public StateSkillConfig Config;
    public RuntimeBlackboard Blackboard = new RuntimeBlackboard();
    public StateRuntimeObserve Observe = new StateRuntimeObserve();
    public int CurrentState;
}
