namespace TahanECS.Entity
{
    public class Entity
    {
        public int Id
        {
            get { return _id; }
        }

        private int _id;

        private EcsWorld _ecsWorld;

        public void Init(EcsWorld world)
        {
            _id = world.CreateEntity();
            _ecsWorld = world;
            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        public void Dispose()
        {
            _ecsWorld.DestroyEntity(_id);
            _ecsWorld = null;
            _id = -1;
        }

        public void SetData<T>(T component) where T : struct
        {
            _ecsWorld.SetComponent(_id, ref component);
        }

        public ref T GetData<T>() where T : struct
        {
            return ref _ecsWorld.GetComponent<T>(_id);
        }
    }
}