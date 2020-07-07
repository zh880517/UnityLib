[System.Serializable]
public abstract class BaseNode
{
    public virtual string Name => GetType().Name;
    public bool IsRoot => false;
    public int MaxCount => -1;
}
