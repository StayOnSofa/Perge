using UnityEngine;

namespace Core
{
    public abstract class Entity
    {
        public Vector3 Position { private set; get; }

        public Entity()
        {
            Position = Vector3.zero;
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
        }

        public abstract void Tick(float dt);
    }
}
