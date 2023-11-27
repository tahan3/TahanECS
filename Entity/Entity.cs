using System;

namespace TahanECS.Entity
{
    public class Entity : IDisposable
    {
        public int Id { get; private set; }

        private EcsWorld _ecsWorld;

        public Entity(EcsWorld world)
        {
            Id = world.CreateEntity();
            _ecsWorld = world;
        }

        public void Dispose()
        {
            _ecsWorld.DestroyEntity(Id);
            _ecsWorld = null;
            Id = -1;
        }

        public void SetData<T>(T component) where T : struct
        {
            _ecsWorld.SetComponent(Id, ref component);
        }

        public ref T GetData<T>() where T : struct
        {
            return ref _ecsWorld.GetComponent<T>(Id);
        }
    }
}