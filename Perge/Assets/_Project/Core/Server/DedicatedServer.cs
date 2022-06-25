using ENet;
using System.Collections.Generic;
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

            _serverWorld = new ServerWorld();
        }

        public override void IncomingConnection(Peer peer)
        {
            Debug.Log("Player Connected");
            
            if (!_players.ContainsKey(peer))
                _players.Add(peer, new ServerPlayer(this, peer, $"Player: {peer.ID}"));
        }
        
        public override void IncomingDisconnection(Peer peer)
        {
            if (_players.ContainsKey(peer))
                _players.Remove(peer);
        }

        public override void Tick(float dt)
        {
            base.Tick(dt);

            foreach (var item in _players)
            {
                item.Value.Tick(dt);
            }

            _serverWorld.Tick(_players.Values);
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
