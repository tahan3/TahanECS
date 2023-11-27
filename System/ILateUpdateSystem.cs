namespace TahanECS.System
{
    public interface ILateUpdateSystem : ISystem
    {
        void OnLateUpdate(int entity);
    }
}