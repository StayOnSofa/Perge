using System;
using System.Collections.Generic;
using Core.Chunks;
using Core.Mono.SortUtils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Mono
{
    public class MonoChunkBuilder
    {
        private const int c_ApplyChunksPerFrame = 3;
        public const int c_RenderRadius = 8;
        
        public static bool IsVisibleFrom(MonoChunkSlice slice, Camera camera) {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, slice.GetBounds());
        }
        
        private Camera _camera;
        private LocalPlayer _player;
        private DistanceComparer _comparer;
        
        public MonoChunkBuilder(LocalPlayer player)
        {
            _player = player;
            _camera = player.GetCamera();
            _comparer = new DistanceComparer(player);
        }

        private List<MonoChunk> _chunkBuffer = new List<MonoChunk>();
        
        private MonoChunk[] GetChunksFromActiveChunkLoader(Dictionary<KeyIndex, MonoChunk> preparedChunks)
        {
            _chunkBuffer.Clear();
            
            KeyIndex key = _player.GetChunkBorder();
            int radius = c_RenderRadius;
            
            for (int x = -radius; x < radius + 2; x++)
            {
                for (int z = -radius; z < radius + 2; z++)
                {
                    int X = key.X + x;
                    int Z = key.Z + z;

                    var index = new KeyIndex(X, Z);

                    if (preparedChunks.ContainsKey(index))
                    {
                        _chunkBuffer.Add(preparedChunks[index]);
                    }
                }
            }

            return _chunkBuffer.ToArray();
        }

        private List<UniTask> _waitBuffer = new List<UniTask>();
        
        public async UniTask BuildChunks(Dictionary<KeyIndex, MonoChunk> preparedChunks)
        {
            await UniTask.SwitchToThreadPool();
            
            _waitBuffer.Clear();
            var array = GetChunksFromActiveChunkLoader(preparedChunks);
            Array.Sort(array, _comparer);
            
            await UniTask.SwitchToMainThread();
            
            foreach (var monoChunk in array)
            {
                var slices = monoChunk.GetSlices();
                
                for (int i = 0; i < slices.Length; i++)
                {
                    var slice = slices[i];
                    if (!slice.IsBuilded())
                    {
                        if (IsVisibleFrom(slice, _camera))
                        {
                            var task = BuildSlice(slice);
                            _waitBuffer.Add(task);

                            if (i % c_ApplyChunksPerFrame == 0)
                            {
                                await UniTask.Yield();
                            }
                        }
                    }
                }
            }

            await UniTask.WhenAll(_waitBuffer);
            await UniTask.SwitchToMainThread();
        }

        private async UniTask BuildSlice(MonoChunkSlice slice)
        {
            slice.ClearLayer();
            await UniTask.SwitchToThreadPool();
            slice.BuildLayer();
            await UniTask.SwitchToMainThread();
            slice.ApplyLayer();
        }
    }
}