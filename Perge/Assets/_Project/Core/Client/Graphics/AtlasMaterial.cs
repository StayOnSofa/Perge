using Core.Mono;
using UnityEngine;

namespace Core.Graphics
{
    public class AtlasMaterial : MonoSingleton<AtlasMaterial>
    {
        private const string c_Shader = "Atlas/Standart";
        
        private static Material _material;
        private static Camera _camera;
        
        private static readonly int c_DistanceTex = Shader.PropertyToID("_DistanceTex");
        private static readonly int c_MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int c_CameraPositon = Shader.PropertyToID("_CameraPositon");

        public Material GetMaterial()
        {
            if (_material == null)
            {
                _material = new Material(Shader.Find(c_Shader));
                
                _material.SetTexture(c_MainTex, Atlas.GetAtlas());
                _material.SetTexture(c_DistanceTex, Atlas.GetBlurAtlas());

                _camera = FindObjectOfType<MonoPlayer>().GetCamera();
            }

            return _material;
        }

        private void Update()
        {
            Vector3 position = _camera.transform.position;
            _material.SetVector(c_CameraPositon, new Vector4(position.x, position.y, position.z, 0));
        }
    }
}