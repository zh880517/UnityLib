namespace LiteECS
{
    public interface IInitializeSystem : ISystem
    {
        void OnInitialize();
    }
}