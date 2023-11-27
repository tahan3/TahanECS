namespace TahanECS.System
{
    public interface IFixedUpdateSystem : ISystem
    {
        void OnFixedUpdate(int entity);
    }
}