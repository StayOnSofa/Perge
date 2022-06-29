using ENet;
using System.Collections.Generic;
using System.Linq;
using Core.PackageUtils;
using Core.PackageUtils.Handlers;
using Core.World;
using UnityEngine;

namespace Core
{
    public class DedicatedServer : Server
    {
        private List<PackageServerHandler> _packageHandlers;
        private Dictionary<Peer, ServerPlayer> _players;

        private ServerWorld _serverWorld;
        
        public DedicatedServer(ushort port, int maxClients = 16) : base(port, maxClients)
        {
            _players = new Dictionary<Peer, ServerPlayer>(maxClients);
            _packageHandlers = new List<PackageServerHandler>();
            
            _packageHandlers.Add(new PackagePlayerStatsHandler(this));

            _serverWorld = new ServerWorld(this);
        }

        public override void IncomingConnection(Peer peer)
        {
            Debug.Log("Player Connected");

            if (!_players.ContainsKey(peer))
            {
                var player = new ServerPlayer(this, peer, $"Player: {peer.ID}");
                _players.Add(peer, player);

                using (var blockStream = new BlockStream(PackageType.PlayerConnect))
                {
                    blockStream.WriteInt(player.PlayerID);
                    
                    foreach (var item in _players)
                    {
                        if (item.Value != player)
                        {
                            SendToPlayer(PacketFlags.Reliable, item.Value, blockStream.ToArray());
                        }
                    }
                }
            }
        }
        
        public override void IncomingDisconnection(Peer peer)
        {
            if (_players.ContainsKey(peer))
            {
                var player = _players[peer];
                _players.Remove(peer);
                
                using (var blockStream = new BlockStream(PackageType.PlayerDisconnect))
                {
                    blockStream.WriteInt(player.PlayerID);
                    
                    foreach (var item in _players)
                    {
                        if (item.Value != player)
                        {
                            SendToPlayer(PacketFlags.Reliable, item.Value, blockStream.ToArray());
                        }
                    }
                }
            }
        }

        public override void Tick(float dt)
        {
            base.Tick(dt);

            foreach (var item in _players)
            {
                item.Value.Tick(dt, _players.Values);
            }

            var array = _players.Values.ToArray();
            _serverWorld.Tick(array);
        }

        public void SendToPlayer(PacketFlags flags, ServerPlayer player, byte[] bytes)
        {
            SendBytes(flags, player.Peer, bytes);
        }

        public override void IncomingReceived(Peer peer, byte[] bytes)
        {
            ServerPlayer player = null;

            if (_players.ContainsKey(peer))
                player = _players[peer];


            if (player != null)
            {
                using (BlockStream stream = new BlockStream(bytes))
                {
                    foreach (var handler in _packageHandlers)
                    {
                        handler.Handle(player, stream);
                    }
                }
            }
        }
    }
}
