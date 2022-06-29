using UnityEngine;

namespace Core
{   
    public class MonoServerPlayer : MonoEntity
    {
        private void Start()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            cube.transform.SetParent(transform);
            cube.transform.localPosition = Vector3.zero;
            
            transform.position = new Vector3(0, 100, 0);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public override void Tick(float dt)
        {
            return;
        }
    }
}