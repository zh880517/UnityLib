public enum StateNodeStatus
{
    None = 0,
    Enter = 1,
    Running = 2,
    ContionFail = 3,
    ContionSucess = 4,
    Exit = 5,
}
/// <summary>
/// 运行监听，用来做debug工具，待完成
/// </summary>
public class StateRuntimeObserve
{
    public void OnVariableChange(string key, float val)
    {

    }

    public void OnNodeStatus(ulong id, StateNodeStatus status)
    {

    }
}
