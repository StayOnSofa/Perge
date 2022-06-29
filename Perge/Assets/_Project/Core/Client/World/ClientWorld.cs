using System.Collections.Generic;
using Core.Chunks;
using Core.Mono;
using Core.PackageUtils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.World
{
    public class ClientWorld : IWorld
    {
        public struct PreparedChunks
        {
            public bool IsDestroyed;
            public Chunk Chunk;
            public KeyIndex KeyIndex;
            
            public PreparedChunks(Chunk chunk)
            {
                IsDestroyed = false;
                Chunk = chunk;
                KeyIndex = chunk.KeyIndex;
            }

            public PreparedChunks(KeyIndex keyIndex)
            {
                IsDestroyed = true;
                Chunk = null;
                KeyIndex = keyIndex;
            }
        }

        private Dictionary<KeyIndex, Chunk> _chunks = new();
        private Dictionary<KeyIndex, MonoChunk> _monoChunksToChunk = new();

        private List<PreparedChunks> _chunksInFrame = new List<PreparedChunks>();
        private PackageChunkHandler _packageChunkHandler;
        private MonoChunkBuilder _monoChunkBuilder;
        public ClientWorld(LocalPlayer player, PackageChunkHandler packageChunkHandler)
        {
            _packageChunkHandler = packageChunkHandler;
            _monoChunkBuilder = new MonoChunkBuilder(player);
        }
        
        public void RemoveChunk(KeyIndex index)
        {
            if (_chunks.ContainsKey(index))
            {
                var chunk = _chunks[index];
                
                _chunks[index].Destroy();
                _chunks.Remove(index);

                MonoDestroy(chunk);
            }
        }

        public void AddChunk(Chunk chunk)
        {
            _chunks.Add(chunk.KeyIndex, chunk);
            MonoCreate(chunk);
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

        public void PlaceBlock(ushort blockID, int x, int y, int z)
        {
            throw new System.NotImplementedException();
        }

        public void BreakBlock(int x, int y, int z)
        {
            throw new System.NotImplementedException();
        }

        private bool _inJob = false;

        private float _everySeconds;
        
        public void Tick()
        {
            _everySeconds += Time.deltaTime;

            if (_everySeconds > IWorld.c_TicksPerSecond)
            {
                if (_inJob == false)
                {
                    _inJob = true;
                    PrepareLoading();
                }

                _everySeconds = 0;
            }
        }

        private async void PrepareLoading()
        {
            await PrepareWorlds();
            _inJob = false;
        }

        private async UniTask PrepareWorlds()
        {
            await PrepareReceivedChunks();
            await _monoChunkBuilder.BuildChunks(_monoChunksToChunk);
        }

        private async UniTask PrepareReceivedChunks()
        {
            if (_packageChunkHandler.Count > 0)
            {
                var byteArray = _packageChunkHandler.PopLayer();

                await UniTask.SwitchToThreadPool();

                _chunksInFrame.Clear();

                foreach (var data in byteArray)
                {
                    using (var blockStream = new BlockStream(data))
                    {
                        if (blockStream.GetPackageType() == PackageType.ChunkUnload)
                        {
                            int X = blockStream.ReadInt();
                            int Z = blockStream.ReadInt();

                            _chunksInFrame.Add(
                                new PreparedChunks(new KeyIndex(X, Z)));
                        }

                        if (blockStream.GetPackageType() == PackageType.ChunkUpload)
                        {
                            Chunk chunk = new Chunk(this, blockStream);

                            _chunksInFrame.Add(
                                new PreparedChunks(chunk));
                        }
                    }
                }

                await UniTask.SwitchToMainThread();

                foreach (var frame in _chunksInFrame)
                {
                    if (frame.IsDestroyed)
                        RemoveChunk(frame.KeyIndex);

                    if (!frame.IsDestroyed)
                        AddChunk(frame.Chunk);
                }

                foreach (var frame in _chunksInFrame)
                {
                    if (!frame.IsDestroyed)
                        frame.Chunk.UpdateHeighbour();
                }
            }
        }

        public void MonoDestroy(Chunk chunk)
        {
            if (_monoChunksToChunk.ContainsKey(chunk.KeyIndex))
            {
                var monoChunk = _monoChunksToChunk[chunk.KeyIndex];
                GameObject.Destroy(monoChunk.gameObject);

                _monoChunksToChunk.Remove(chunk.KeyIndex);
            }
        }

        public void MonoCreate(Chunk chunk)
        {
            if (_monoChunksToChunk.ContainsKey(chunk.KeyIndex))
            {
                var monoChunk = _monoChunksToChunk[chunk.KeyIndex];
                GameObject.Destroy(monoChunk.gameObject);
            }

            var monochunk = new GameObject("Chunk").AddComponent<MonoChunk>();
            monochunk.Constructor(chunk);
            
            _monoChunksToChunk.Add(chunk.KeyIndex, monochunk);
        }
    }
}