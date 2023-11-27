using System;

namespace TahanECS.Component
{
    public class ComponentPool<T> : IComponentPool where T : struct
    {
        private const int PoolSize = 256;
        private Component[] _components = new Component[PoolSize];
        private int _size;

        void IComponentPool.AllocateComponent()
        {
            if (_size + 1 >= _components.Length)
            {
                Array.Resize(ref _components, _components.Length * 2);
            }

            _components[_size] = new Component
            {
                Exists = false,
                Value = default
            };

            _size++;
        }

        public ref T GetComponent(int entity) //Index
        {
            ref var component = ref _components[entity];
            return ref component.Value;
        }

        public void SetComponent(int entity, ref T data)
        {
            ref var component = ref _components[entity];
            component.Exists = true;
            component.Value = data;
        }

        public bool HasComponent(int entity)
        {
            return _components[entity].Exists;
        }

        public void RemoveComponent(int entity)
        {
            ref var component = ref _components[entity];
            component.Exists = false;
        }

        private struct Component
        {
            public bool Exists;
            public T Value;
        }
    }
}