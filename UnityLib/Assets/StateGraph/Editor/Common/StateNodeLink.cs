using System;
[Serializable]
public class StateNodeLink
{
    public StateNodeRef From;
    public StateNodeRef To;
    public bool IsChild;
}