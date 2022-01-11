namespace LiteECS
{
    public interface IComponent
    {
    }

    public interface IReset
    {

        //重置成员变量为初始值，防止重用时逻辑错误
        void Reset();
    }
}

