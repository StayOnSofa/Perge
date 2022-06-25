using UnityEngine;

namespace Core
{
    public abstract class MonoEntity : MonoBehaviour
    {
        public static T Constructor<T>(Entity entity) where T : MonoEntity
        {
            GameObject gameObject = new GameObject($"[{typeof(T).Name}]");
            T mObject = gameObject.AddComponent<T>();

            mObject.Init(entity);
            
            return mObject;
        }


        private Entity _entity;
        
        public void Init(Entity entity)
        {
            _entity = entity;
        }

        public void Update()
        {
            float dt = Time.deltaTime;
            _entity.Tick(dt);

            Tick(dt);
        }

        public abstract void Tick(float dt);
    }
}