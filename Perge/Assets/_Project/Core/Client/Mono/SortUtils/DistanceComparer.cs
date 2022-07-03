using System.Collections.Generic;
using Core.Chunks;
using UnityEngine;

namespace Core.Mono.SortUtils
{
    public class DistanceComparer : IComparer<MonoChunk>
    {
        private Vector2 _traget;
        private LocalPlayer _localPlayer;
        
        public DistanceComparer(LocalPlayer localPlayer)
        {
            _traget = new Vector2(localPlayer.Position.x, localPlayer.Position.z);
            _localPlayer = localPlayer;
        }

        public int Compare(MonoChunk a, MonoChunk b)
        {
            Vector2 aPosition = new Vector2(a.KeyIndex.X * Chunk.Scale, a.KeyIndex.Z * Chunk.Scale);
            Vector2 bPosition = new Vector2(b.KeyIndex.X * Chunk.Scale, b.KeyIndex.Z * Chunk.Scale);
            
            _traget = new Vector2(_localPlayer.Position.x, _localPlayer.Position.z);
            
            return Vector2.Distance(aPosition, _traget).CompareTo(Vector3.Distance(bPosition, _traget));
        }
    }
}