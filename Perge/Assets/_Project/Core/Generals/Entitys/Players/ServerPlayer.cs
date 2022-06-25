using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Chunks;
using Core.Mono;
using Core.PackageUtils;
using ENet;
using UnityEngine;

namespace Core
{
    public class ServerPlayer : Player
    {
        private Queue<byte[]> _bytesQueue;
        
        public Peer Peer { private set; get; }
        private DedicatedServer _server;

        private List<Chunk> _prevOrder;
        
        public KeyIndex GetChunkBorder()
        {
            return KeyIndex.ToChunkCoordinates((int)Position.x, (int)Position.z);
        }

        public ServerPlayer(DedicatedServer server, Peer peer, string name) : base(name)
        {
            Peer = peer;
            _server = server;
            _bytesQueue = new Queue<byte[]>();
            _prevOrder = new List<Chunk>();
        }

        private bool _inJob = false;

        public bool InWork()
        {
            return _inJob;
        }

        public void ApplyChunk(IEnumerable<Chunk> chunks)
        {
            _inJob = true;
            MonoCoroutine.Instance.Play(CalculateSendingChunks(chunks));
        }

        private IEnumerator CalculateSendingChunks(IEnumerable<Chunk> chunks)
        {
            yield return MonoThreading.Instance.ThreadAsync(() =>
            {
                Chunk[] newOrder = chunks.ToArray();

                foreach (var chunk in newOrder)
                {
                    if (!_prevOrder.Contains(chunk))
                    {
                        using (var blockStream = new BlockStream(PackageType.ChunkUpload))
                        {
                            chunk.Write(blockStream);
                            _bytesQueue.Enqueue(blockStream.ToArray());
                        }
                        
                        _prevOrder.Add(chunk);
                    }
                }

                for (int i = 0; i < _prevOrder.Count; i++)
                {
                    var chunk = _prevOrder[i];
                    if (!newOrder.Contains(chunk))
                    {
                        using (var blockStream = new BlockStream(PackageType.ChunkUnload))
                        {
                            blockStream.WriteInt(chunk.KeyIndex.X);
                            blockStream.WriteInt(chunk.KeyIndex.Z);
                            
                            _bytesQueue.Enqueue(blockStream.ToArray());
                        }
                        
                        _prevOrder.RemoveAt(i);
                    }
                }
                
            });
            

            _inJob = false;
        }

        public override void Tick(float dt)
        {
            if (_inJob == false)
            {
                if (_bytesQueue.Count > 0)
                {
                    var bytes = _bytesQueue.Dequeue();
                    _server.SendToPlayer(PacketFlags.Reliable, this, bytes);
                }
            }
        }
    }
}