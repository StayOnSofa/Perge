using Core.Chunks;
using UnityEngine;

namespace Core.Mono
{ 
    public class MonoChunk : MonoBehaviour
    {
        private Chunk _chunk;
        public KeyIndex KeyIndex { private set; get; }

        private MonoChunkSlice[] _slices;
        
        public void Constructor(Chunk chunk)
        {
            _chunk = chunk;
            _slices = new MonoChunkSlice[Chunk.Height / Chunk.Scale];
            
            KeyIndex = _chunk.KeyIndex;
            
            transform.position = new Vector3(
                KeyIndex.X * Chunk.Scale, 0, KeyIndex.Z * Chunk.Scale);

            PrepareSlices();
            _chunk.OnUpdate += ChunkUpdateLayer;
        }

        private void ChunkUpdateLayer(bool isFull, int y)
        {
            if (isFull)
            {
                for (int i = 0; i < _slices.Length; i++)
                {
                    _slices[i].UpdateLayer();
                }
            }
            else
            {
                int layer = y % Chunk.Scale;
                _slices[layer].UpdateLayer();
            }
        }
        
        private void PrepareSlices()
        {
            for (int i = 0; i < _slices.Length; i++)
            {
                int layer = i;
                
                var monoObject = new GameObject("Slice");
                monoObject.transform.SetParent(transform);
                
                monoObject.transform.localPosition 
                    = new Vector3(0, layer * Chunk.Scale, 0);

                var monoSlice = monoObject.AddComponent<MonoChunkSlice>();
                monoSlice.Create(_chunk, layer);
                
                _slices[i] = monoSlice;
            }
        }

        public MonoChunkSlice[] GetSlices()
        {
            return _slices;
        }

        public Bounds GetBounds()
        {
            Vector3 position = transform.position;
            Vector3 scale = new Vector3(Chunk.Scale, Chunk.Height, Chunk.Scale);

            return new Bounds((position + scale/2) - Vector3.one/2, scale);
        }
        
        private void OnDrawGizmos()
        {
            var bounds = GetBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
        }
        
        public bool IsDead()
        {
            return _chunk.IsDestroyed();
        }

        private void OnDestroy()
        {
            if (_chunk != null)
            {
                _chunk.OnUpdate -= ChunkUpdateLayer;
            }
        }
    }
}