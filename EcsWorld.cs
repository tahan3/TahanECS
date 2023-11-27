using System;
using System.Collections.Generic;
using System.Reflection;
using TahanECS.Component;
using TahanECS.System;

namespace TahanECS
{
    public sealed class EcsWorld
    {
        private readonly List<ISystem> _systems = new();
        private readonly List<IUpdateSystem> _updateSystems = new();
        private readonly List<IFixedUpdateSystem> _fixedUpdateSystems = new();
        private readonly List<ILateUpdateSystem> _lateUpdateSystems = new();

        private readonly Dictionary<Type, IComponentPool> _componentPools = new();
        private readonly List<bool> _entities = new();

        public int CreateEntity()
        {
            var id = 0;
            var count = _entities.Count;

            for (; id < count; id++)
            {
                if (!_entities[id])
                {
                    _entities[id] = true;
                    return id;
                }
            }

            id = count;
            _entities.Add(true);

            foreach (var pool in _componentPools.Values)
            {
                pool.AllocateComponent();
            }

            return id;
        }

        public void DestroyEntity(int entity)
        {
            _entities[entity] = false;
            foreach (var pool in _componentPools.Values)
            {
                pool.RemoveComponent(entity);
            }
        }

        public ref T GetComponent<T>(int entity) where T : struct
        {
            var pool = (ComponentPool<T>) _componentPools[typeof(T)];
            return ref pool.GetComponent(entity);
        }

        public void SetComponent<T>(int entity, ref T component) where T : struct
        {
            var pool = (ComponentPool<T>) _componentPools[typeof(T)];
            pool.SetComponent(entity, ref component);
        }

        public void Update()
        {
            for (int i = 0, count = _updateSystems.Count; i < count; i++)
            {
                var system = _updateSystems[i];
                for (var entity = 0; entity < _entities.Count; entity++)
                {
                    if (_entities[entity])
                    {
                        system.OnUpdate(entity);
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            for (int i = 0, count = _fixedUpdateSystems.Count; i < count; i++)
            {
                var system = _fixedUpdateSystems[i];
                for (var entity = 0; entity < _entities.Count; entity++)
                {
                    if (_entities[entity])
                    {
                        system.OnFixedUpdate(entity);
                    }
                }
            }
        }

        public void LateUpdate()
        {
            for (int i = 0, count = _lateUpdateSystems.Count; i < count; i++)
            {
                var system = _lateUpdateSystems[i];
                for (var entity = 0; entity < _entities.Count; entity++)
                {
                    if (_entities[entity])
                    {
                        system.OnLateUpdate(entity);
                    }
                }
            }
        }

        public void BindComponent<T>() where T : struct
        {
            _componentPools[typeof(T)] = new ComponentPool<T>();
        }

        public void BindSystem<T>() where T : ISystem, new()
        {
            var system = new T();
            _systems.Add(system);

            if (system is IUpdateSystem updateSystem)
            {
                _updateSystems.Add(updateSystem);
            }

            if (system is IFixedUpdateSystem fixedUpdateSystem)
            {
                _fixedUpdateSystems.Add(fixedUpdateSystem);
            }

            if (system is ILateUpdateSystem lateUpdateSystem)
            {
                _lateUpdateSystems.Add(lateUpdateSystem);
            }
        }

        public void Install()
        {
            foreach (var system in _systems)
            {
                Type systemType = system.GetType();
                var fields = systemType.GetFields(
                    BindingFlags.Instance |
                    BindingFlags.DeclaredOnly |
                    BindingFlags.NonPublic
                );
                foreach (var field in fields)
                {
                    var componentType = field.FieldType;
                    var componentPool = _componentPools[componentType];
                    field.SetValue(system, componentPool);
                }
            }
        }
    }
}