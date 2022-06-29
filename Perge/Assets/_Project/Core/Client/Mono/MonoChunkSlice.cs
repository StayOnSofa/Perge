using UnityEngine;
using Core.Chunks;
using Core.Graphics;
using Vector3 = UnityEngine.Vector3;

namespace Core.Mono
{
    public class MonoChunkSlice : MonoBehaviourAtlas
    {
        private int _layer;
        
        private MeshChunk _meshChunk;
        private Chunk _chunk;

        private bool _isBuilded = false;
        
        public void Create(Chunk chunk, int layer)
        {
            _chunk = chunk;
            _meshChunk = new MeshChunk(_chunk);
            
            _layer = layer;
            
            SetMesh(_meshChunk.GetMesh());

            Vector3 position = transform.localPosition;
            position.y = layer * Chunk.Scale;

            transform.localPosition = position;
        }

        public Bounds GetBounds()
        {
            Vector3 position = transform.position;
            Vector3 scale = new Vector3(Chunk.Scale, Chunk.Scale, Chunk.Scale);

            return new Bounds((position + scale/2) - Vector3.one/2, scale);
        }

        public bool IsBuilded()
        {
            return _isBuilded;
        }

        private void OnDestroy()
        {
            Destroy(_meshChunk?.GetMesh());
            _chunk = null;
            _meshChunk = null;
        }

        public void UpdateLayer()
        {
            _isBuilded = false;
        }

        public void ClearLayer()
        {
            _meshChunk.ClearMesh();
        }

        public void BuildLayer()
        {
            _meshChunk.Build(_layer);
        }

        public void ApplyLayer()
        {
            _meshChunk.Apply();
            _isBuilded = true;
        }
    }
}