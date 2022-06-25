using Core.Mono;
using UnityEngine;

namespace Core.Graphics
{
    public class AtlasMaterial : MonoSingleton<AtlasMaterial>
    {
        private static Material _material;
        private static Camera _camera;
        
        public Material GetMaterial()
        {
            if (_material == null)
            {
                _material = new Material(Shader.Find("Atlas/Standart"));
                
                _material.SetTexture("_MainTex", Atlas.GetAtlas());
                _material.SetTexture("_DistanceTex", Atlas.GetBlurAtlas());

                _camera = FindObjectOfType<Camera>();
            }

            return _material;
        }

        private void Update()
        {
            Vector3 position = _camera.transform.position;
            _material.SetVector("_CameraPositon", new Vector4(position.x, position.y, position.z, 0));
        }
    }
}