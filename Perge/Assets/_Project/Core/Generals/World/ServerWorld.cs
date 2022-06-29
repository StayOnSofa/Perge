using System.Collections.Generic;
using Core.Biomes;
using Core.Chunks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.World
{
    public class ServerWorld : IWorld
    {
        private Dictionary<KeyIndex, Chunk> _chunks;
        
        private List<Chunk> _chunkToDestroy;
        
        private BiomeHandler _biomeHandler;

        private DedicatedServer _server;
        public ServerWorld(DedicatedServer server)
        {
            _server = server;
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
        
        public Vector2Int ToChunkCoordinates(int globalX, int globalZ)
        {
            int chunkX = (int)Mathf.Floor(globalX / Chunk.Scale);
            int chunkZ = (int)Mathf.Floor(globalZ / Chunk.Scale);

            return new Vector2Int(chunkX, chunkZ);
        }

        public Vector2Int ToBlockCords(int globalX, int globalZ)
        {
            var key = KeyIndex
                .ToChunkCoordinates(globalX, globalZ);
            
            int blockX = (int)(globalX - (key.X * Chunk.Scale));   
            int blockZ = (int)(globalZ - (key.Z * Chunk.Scale));

            return new Vector2Int(blockX, blockZ);
        }

        public void PlaceBlock(ushort blockID, int x, int y, int z)
        {
            var key = KeyIndex
                .ToChunkCoordinates(x, z);
            
            var chunk = GetOrCreate(key.X, key.Z);
            var localBlock = ToBlockCords(x, z);
            
            chunk.PlaceBlock(blockID, localBlock.x, y, localBlock.y);
        }

        public void BreakBlock(int x, int y, int z)
        {
            var key = KeyIndex
                .ToChunkCoordinates(x, z);
            
            var chunk = GetOrCreate(key.X, key.Z);
            var localBlock = ToBlockCords(x, z);
            
            chunk.BreakBlock(localBlock.x, y, localBlock.y);
        }

        public bool HasChunk(KeyIndex index)
        {
            return _chunks.ContainsKey(index);
        }

        private Chunk GetOrCreate(int x, int z)
        {
            var key = new KeyIndex(x, z);

            if (HasChunk(key))
            {
                return _chunks[key];
            }

            var chunk = new Chunk(this, key, _biomeHandler, false);
            _chunks.Add(key, chunk);
            
            return chunk;
        }
        
        private void CollectChunkToRemoveList()
        {
            foreach (Chunk chunk in _chunks.Values)
            {
                _chunkToDestroy.Add(chunk);
            }
        }
        
        private void DeleteFromRemoveList(Chunk chunk)
        {
            if (_chunkToDestroy.Contains(chunk))
            {
                _chunkToDestroy.Remove(chunk);
            }
        }

        private List<Chunk> _chunkBuffer = new List<Chunk>();
        private Chunk[] GetChunksFromActiveChunkLoader(ServerPlayer serverPlayer)
        {
            _chunkBuffer.Clear();
            
            KeyIndex key = serverPlayer.GetChunkBorder();
            int radius = serverPlayer.GetSquareLoading();
            
            for (int x = -radius; x < radius + 2; x++)
            {
                for (int z = -radius; z < radius + 2; z++)
                {
                    int X = key.X + x;
                    int Z = key.Z + z;

                    Chunk chunk = GetOrCreate(X, Z);
                    _chunkBuffer.Add(chunk);
                }
            }

            return _chunkBuffer.ToArray();
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

        private float _everySeconds;
        
        public void Tick(IEnumerable<ServerPlayer> players)
        {
            _everySeconds += Time.deltaTime;

            if (_everySeconds > IWorld.c_TicksPerSecond)
            {
                if (_inJob == false)
                {
                    _inJob = true;
                    PrepareLoading(players);
                }

                _everySeconds = 0;
            }
        }
        
        private async void PrepareLoading(IEnumerable<ServerPlayer> players)
        {
            await ThreadAsync(players);
            _inJob = false;
        }

        private async UniTask ThreadAsync(IEnumerable<ServerPlayer> players)
        {
            ClearUnusedChunksList();
            
            await UniTask.SwitchToThreadPool();
            
            CollectChunkToRemoveList();

            foreach (var player in players)
            {
                 var chunks = GetChunksFromActiveChunkLoader(player);

                foreach (var chunk in chunks)
                {
                    DeleteFromRemoveList(chunk);
                    chunk.BuildStructure();
                }

                if (!player.InJob())
                {
                    if (player.IsChangeBorder())
                    {
                        player.ApplyChunks(chunks);
                    }
                }
            }
            
            await UniTask.SwitchToMainThread();
        }
    }
}