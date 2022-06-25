using System.Collections.Generic;
using Core.Biomes;
using Core.Chunks;
using Core.Mono;
using UnityEngine;

namespace Core.World
{
    public class ClientWorld : IWorld
    {
        private BiomeHandler _biomeHandler = new BiomeHandler();
        
        private Dictionary<KeyIndex, Chunk> _chunks 
            = new Dictionary<KeyIndex, Chunk>();
        
        public void AddChunk(int x, int z)
        {
            var key = new KeyIndex(x, z);
            var chunk = new Chunk(this, key, _biomeHandler);
            
            _chunks.Add(key, chunk);
            
            СhunkСreated(chunk);
        }

        public void RemoveChunk(KeyIndex index)
        {
            if (_chunks.TryGetValue(index, out Chunk chunk))
            {
                chunk.Destroy();
                _chunks.Remove(index);
            }
        }

        public void AddChunk(Chunk chunk)
        { 
            _chunks.Add(chunk.KeyIndex, chunk);
            СhunkСreated(chunk);
        }

        public Chunk GetChunk(int x, int z)
        {
            var key = new KeyIndex(x, z);
            if (_chunks.ContainsKey(key))
            {
                return _chunks[key];
            }
            
            return null;
        }

        public void СhunkСreated(Chunk chunk)
        {
            var monochunk = new GameObject("Chunk").AddComponent<MonoChunk>();
            monochunk.Create(chunk);
        }
    }
}