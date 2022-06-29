using System.Collections.Generic;
using Core.PackageUtils;
using UnityEngine;

namespace Core.World
{
    public class PackageChunkHandler
    {
        public Queue<byte[]> _bytesQueue 
            = new Queue<byte[]>();

        public void Receive(byte[] bytes, BlockStream stream)
        {
            if (stream.GetPackageType() == PackageType.ChunkUnload)
                _bytesQueue.Enqueue(bytes);
            
            if (stream.GetPackageType() == PackageType.ChunkUpload)
                _bytesQueue.Enqueue(bytes);
        }

        public int Count => _bytesQueue.Count;
        
        public IEnumerable<byte[]> PopLayer()
        {
            var array = _bytesQueue.ToArray();
            
            _bytesQueue.Clear();

            return array;
        }
    }
}