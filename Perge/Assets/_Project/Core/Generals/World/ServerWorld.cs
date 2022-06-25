using System.Collections;
using System.Collections.Generic;
using Core.Biomes;
using Core.Chunks;
using Core.Mono;
using UnityEngine;

namespace Core.World
{
    public class ServerWorld : IWorld
    {
        private const int _loadingSquare = 10;
        
        private Dictionary<KeyIndex, Chunk> _chunks;
        private List<Chunk> _chunkToDestroy;
        
        private BiomeHandler _biomeHandler;
        
        public ServerWorld()
        {
            _chunks = new Dictionary<KeyIndex, Chunk>();
            _chunkToDestroy = new List<Chunk>();
            
            _biomeHandler = new BiomeHandler();
        }

        public Chunk GetChunk(int x, int z)
        {
            var key = new KeyIndex(x, z);
            if (HasChunk(key))
            {
                return _chunks[key];
            }

            return null;
        }
        
        public bool HasChunk(int x, int z)
        {
            return _chunks.ContainsKey(new KeyIndex(x, z));
        }

        public bool HasChunk(KeyIndex index)
        {
            return _chunks.ContainsKey(index);
        }

        private Chunk GetOrCreate(int x, int z)
        {
            var key = new KeyIndex(x, z);

            if (HasChunk(key))
                return _chunks[key];

            var chunk = new Chunk(null, key, _biomeHandler);
            _chunks.Add(key, chunk);
            
            return chunk;
        }

        public void СhunkСreated(Chunk chunk)
        {
            return;
        }
        
        private void DeleteFromRemoveList(Chunk chunk)
        {
            Debug.LogWarning(_chunkToDestroy.Count);
            
            if (_chunkToDestroy.Contains(chunk))
            {
                _chunkToDestroy.Remove(chunk);
            }
        }
        
        private void CollectChunkToRemoveList()
        {
            foreach (Chunk chunk in _chunks.Values)
            {
                _chunkToDestroy.Add(chunk);
            }
        }

        public IEnumerable<Chunk> GetChunksFromActiveChunkLoader(ServerPlayer serverPlayer, int radius)
        {
            Chunk[] chunks = new Chunk[((radius-1) + (radius-1)) * ((radius-1) + (radius-1))];
            KeyIndex key = serverPlayer.GetChunkBorder();

            int index = 0;

            for (int x = -radius; x < radius; x++)
            {
                for (int z = -radius; z < radius; z++)
                {
                    int X = key.X + x;
                    int Z = key.Z + z;

                    Chunk chunk = GetOrCreate(X, Z);
                    DeleteFromRemoveList(chunk);

                    if (x > -radius && x < radius - 1)
                    {
                        if (z > -radius && z < radius - 1)
                        {
                            chunks[index] = chunk;
                            index++;
                        }
                    }
                }
            }

            return chunks;
        }
        
        private void ClearUnusedChunksList()
        {
            foreach (Chunk chunk in _chunkToDestroy)
            {
                chunk.Destroy();
                _chunks.Remove(chunk.KeyIndex);
            }

            _chunkToDestroy.Clear();
        }

        private bool _inJob = false;
        
        public void Tick(IEnumerable<ServerPlayer> players)
        {
            if (_inJob == false)
            {
                _inJob = true;
                MonoCoroutine.Instance.Play(AsyncTick(players));
            }
        }

        private IEnumerator AsyncClearTick()
        {
            yield return MonoThreading.Instance.ThreadAsync(() =>
            {
                ClearUnusedChunksList();
                CollectChunkToRemoveList();
            });
        }
        
        private IEnumerator AsyncTick(IEnumerable<ServerPlayer> players)
        {
            yield return AsyncClearTick();
            
            foreach (var player in players)
            {
                IEnumerable<Chunk> chunks = null;

                    yield return MonoThreading.Instance.ThreadAsync(() =>
                    {
                        chunks = GetChunksFromActiveChunkLoader(player, _loadingSquare);
                    });

                    if (!player.InWork())
                    {
                        player.ApplyChunk(chunks);
                    }
            }
            
            _inJob = false;
        }

    }
}