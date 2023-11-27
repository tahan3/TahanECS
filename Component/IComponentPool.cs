namespace TahanECS.Component
{
    public interface IComponentPool
    {
        void AllocateComponent();

        void RemoveComponent(int entity);
    }
}