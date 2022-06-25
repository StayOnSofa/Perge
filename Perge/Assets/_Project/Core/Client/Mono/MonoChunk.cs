using Core.Chunks;
using UnityEngine;

namespace Core.Mono
{ 
    public class MonoChunk : MonoBehaviour
    {
        private MonoChunkSlice[] _chunks = new MonoChunkSlice[Chunk.Height/Chunk.Scale];

        private bool _hasUpdate = false;
        private bool _hasApply = true;
        
        private Chunk _chunk;

        private Vector3 _position;

        public KeyIndex KeyIndex { private set; get; }

        public void Constructor(Chunk chunk)
        {
            _chunk = chunk;
            _chunk.OnUpdate += Rebuild;
            
            MonoChunkBuilder
                .Instance.AddChunk(chunk.KeyIndex, this);

            _position = transform.position;

            KeyIndex = chunk.KeyIndex;
        }

        private void Rebuild()
        {
            _hasUpdate = false;
        }

        public bool IsUpdated()
        {
            return _hasUpdate;
        }

        public bool HasApply()
        {
            return _hasApply;
        }

        private MonoChunkSlice CreateSlice()
        {
            var gameObject = new GameObject("Slice");
            
            gameObject.transform.SetParent(transform);
            gameObject.transform.localPosition = Vector3.zero;
            
            return gameObject.AddComponent<MonoChunkSlice>();
        }

        public void UpdateLayer()
        {
            for (int i = 0; i < _chunks.Length; i++)
            {
                MonoChunkSlice slice = _chunks[i];
                if (slice != null)
                {
                   slice.UpdateLayer();
                }
            }
            
            _hasUpdate = true;
            _hasApply = false;
        }
        
        public void ClearLayer()
        {
            for (int i = 0; i < _chunks.Length; i++)
            {
                MonoChunkSlice slice = _chunks[i];
                if (slice != null)
                {
                    slice.ClearLayer();
                }
            }
        }
        
        public void ApplyLayer()
        {
            for (int i = 0; i < _chunks.Length; i++)
            {
                MonoChunkSlice slice = _chunks[i];
                if (slice != null)
                {
                    slice.ApplyLayer();
                }
            }

            _hasApply = true;
        }

        public void Create(Chunk chunk)
        {
            transform.position 
                = new Vector3(chunk.KeyIndex.X, 0, chunk.KeyIndex.Z) * Chunk.Scale;
            
            for (int i = 0; i < _chunks.Length; i++)
            {
                MonoChunkSlice slice = CreateSlice();
                slice.Create(chunk, i);
                
                _chunks[i] = slice;
            }

            Constructor(chunk);
        }

        public bool IsVisible()
        {
            for (int i = 0; i < _chunks.Length; i++)
            {
                MonoChunkSlice slice = _chunks[i];
                if (slice != null)
                {
                    if (slice.IsVisible())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnDestroy()
        {
            _chunk.OnUpdate -= Rebuild;
            
            for (int i = 0; i < _chunks.Length; i++)
            {
                MonoChunkSlice slice = _chunks[i];
                if (slice != null)
                {
                    Destroy(slice.gameObject);
                }
            }
        }

        public bool IsDead()
        {
            return _chunk.IsDestroyed();
        }

        public Vector3 GetPosition()
        {
            return _position;
        }
    }
}