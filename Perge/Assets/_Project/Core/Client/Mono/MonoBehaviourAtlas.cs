using System;
using Core.Graphics;
using UnityEngine;

namespace Core.Mono
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public abstract class MonoBehaviourAtlas : MonoBehaviour
    {
        private static Material GetMaterial()
        {
            return AtlasMaterial.Instance.GetMaterial();
        }
        
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private void Start()
        {
            Constructor();
        }

        public void SetMesh(Mesh mesh)
        {
            if (_meshFilter == null)
                _meshFilter = GetComponent<MeshFilter>();
            
            _meshFilter.mesh = mesh;
        }

        public bool IsVisible()
        {
            if (_meshRenderer == null)
                _meshRenderer = GetComponent<MeshRenderer>();
            
            return _meshRenderer.isVisible;
        }

        public virtual void Constructor()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.material = GetMaterial();
        }
    }
}