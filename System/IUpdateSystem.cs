namespace TahanECS.System
{
    public interface IUpdateSystem : ISystem
    {
        void OnUpdate(int entity);
    }
}