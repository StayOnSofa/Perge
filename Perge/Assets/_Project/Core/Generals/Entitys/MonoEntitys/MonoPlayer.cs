using System;
using Core.Chunks;
using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(FreeCam)), RequireComponent(typeof(Camera))]
    public class MonoPlayer : MonoEntity
    {
        private void Start()
        {
            transform.position = new Vector3(0, 100, 0);
        }

        public override void Tick(float dt)
        {
            return;
        }

        public Camera GetCamera()
        {
            return GetComponent<Camera>();
        }
    }
}