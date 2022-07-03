using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Chunks;
using Core.PackageUtils;
using Cysharp.Threading.Tasks;
using ENet;
using UnityEngine;

namespace Core
{
    public class ServerPlayer : Player
    {
        public static int s_PlayerIDs = 0;
        
        public const int c_SquareLoading = 10;
        
        public Peer Peer { private set; get; }
        private DedicatedServer _server;

        private List<Chunk> _usedChunks;

        public int PlayerID { private set; get;}

        public KeyIndex GetChunkBorder()
        {
            return KeyIndex
                .ToChunkCoordinates((int)Position.x, (int)Position.z);
        }

        private KeyIndex _prevBorder = new(-Int32.MaxValue, -Int32.MaxValue);
        
        public bool IsChangeBorder()
        {
            return !_prevBorder.Equals(GetChunkBorder());
        }
        
        public int GetSquareLoading()
        {
            return c_SquareLoading;
        }

        public ServerPlayer(DedicatedServer server, Peer peer, string name) : base(name)
        {
            Peer = peer;
            _server = server;
            _usedChunks = new List<Chunk>();

            PlayerID = s_PlayerIDs;
            s_PlayerIDs += 1;
        }

        private bool _inJob = false;

        public bool InJob() => _inJob;
        
        public async void ApplyChunks(IEnumerable<Chunk> chunks)
        {
            _inJob = true;
            await SendingJobs(chunks);
        }

        private Queue<byte[]> _byteBuffer = new Queue<byte[]>();
        
        private async UniTask SendingJobs(IEnumerable<Chunk> newOrder)
        {
            await UniTask.SwitchToThreadPool();
            
            Chunk[] newOrders = newOrder.ToArray();
            
            foreach (var chunk in _usedChunks.ToArray())
            {
                if (!newOrders.Contains(chunk))
                {
                    _usedChunks.Remove(chunk);
                    
                    using (var blockStream = new BlockStream(PackageType.ChunkUnload))
                    {
                        var key = chunk.KeyIndex;
                        
                        blockStream.WriteInt(key.X);
                        blockStream.WriteInt(key.Z);
                            
                        var bytes = blockStream.ToArray();
                        _byteBuffer.Enqueue(bytes);
                    }
                }
            }

            int lenght = 0;
            
            foreach (var chunk in newOrders)
            {
                if (_usedChunks.Contains(chunk)) continue;
                _usedChunks.Add(chunk);
                
                using (var blockStream = new BlockStream(PackageType.ChunkUpload))
                {
                    chunk.Write(blockStream);

                    var bytes = blockStream.ToArray();

                    lenght = bytes.Length;
                    _byteBuffer.Enqueue(bytes);
                }
            }
            
            await UniTask.SwitchToMainThread();

            int index = 0;
            
            while (_byteBuffer.Count > 0)
            {
                index += 1;
                
                var bytes = _byteBuffer.Dequeue();
                _server.SendToPlayer(PacketFlags.Reliable, this, bytes);

                if (index % 4 == 0)
                {
                    await Task.Yield();
                }
            }
            
            Debug.Log("BufferScale: " + lenght);
            
            _inJob = false;
            _prevBorder = GetChunkBorder();
        }

        public void Tick(float dt, IEnumerable<ServerPlayer> serverPlayers)
        {
            foreach (var player in serverPlayers)
            {
                if (player != this)
                {
                    using (var blockStream = new BlockStream(PackageType.PlayerPosition))
                    {
                        blockStream.WriteInt(PlayerID);
                        blockStream.WriteVector3(Position);
                        _server.SendToPlayer(PacketFlags.Instant, player, blockStream.ToArray());
                    }
                }
            }
        }

        public override void Tick(float dt)
        {
            return;
        }
    }
}