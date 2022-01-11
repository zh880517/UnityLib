
namespace LiteECS
{
    public interface ICleanupSystem : ISystem
    {
        void OnCleanup();
    }
}