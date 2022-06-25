using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(FreeCam)), RequireComponent(typeof(Camera))]
    public class MonoPlayer : MonoEntity
    {
        public override void Tick(float dt)
        {
            return;
        }
    }
}