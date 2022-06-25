using UnityEngine;
using Core.Chunks;
using Core.Graphics;

namespace Core.Mono
{
    public class MonoChunkSlice : MonoBehaviourAtlas, ISubscriberChunk
    {
        private int _layer;
        
        private MeshChunk _meshChunk;
        private Chunk _chunk;
        public void Create(Chunk chunk, int layer)
        {
            _chunk = chunk;
            _meshChunk = new MeshChunk(_chunk);

            _chunk.Subscribe(this);

            _layer = layer;
            
            SetMesh(_meshChunk.GetMesh());

            Vector3 position = transform.localPosition;
            position.y = layer * Chunk.Scale;

            transform.localPosition = position;
        }

        private void OnDestroy()
        {
            _chunk?.Unsubscribe(this);
            Destroy(_meshChunk?.GetMesh());
        }

        public void UpdateLayer(int y)
        {
            int sLayer = _layer * Chunk.Scale;
            int eLayer = sLayer + Chunk.Scale;

            if (y >= sLayer && y < eLayer)
            {
                UpdateLayer();
                ApplyLayer();
            }
        }

        public void ClearLayer()
        {
            _meshChunk.ClearMesh();
        }

        public void UpdateLayer()
        {
            _meshChunk.Build(_layer);
        }

        public void ApplyLayer()
        {
            _meshChunk.Apply();
        }
    }
}