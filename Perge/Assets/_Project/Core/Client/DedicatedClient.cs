using System.Collections.Generic;
using Core.PackageUtils;
using Core.World;
using Unity.VisualScripting;
using UnityEngine;

namespace Core
{
    public class DedicatedClient : Client
    {
        private LocalPlayer _localPlayer;
        private ClientWorld _clientWorld;

        private PackageChunkHandler _packageChunkHandler;

        private Dictionary<int, MonoServerPlayer> _monoServerPlayer = new();

        public DedicatedClient(string ip, ushort port) : base(ip, port)
        {
            _packageChunkHandler = new PackageChunkHandler();
            Debug.Log("[Client] Start");
        }

        public override void IConnected()
        {
            Debug.Log("[Client] Connected");
            
            _localPlayer = new LocalPlayer(this);
            _clientWorld = new ClientWorld(_localPlayer, _packageChunkHandler);
        }

        public override void IDisconnected()
        {
            Debug.Log("[Client] Disconnected");
        }
        
        public override void Tick(float dt)
        {
            base.Tick(dt);
            _clientWorld?.Tick();
        }

        public override void IReceived(byte[] bytes)
        {
            using (BlockStream stream = new BlockStream(bytes))
            {
                _packageChunkHandler.Receive(bytes, stream);

                if (stream.GetPackageType() == PackageType.PlayerConnect)
                {
                    int id = stream.ReadInt();

                    if (!_monoServerPlayer.ContainsKey(id))
                    {
                        var mono = MonoEntity.Constructor<MonoServerPlayer>(null);
                        _monoServerPlayer.Add(id, mono);
                    }
                }
                
                if (stream.GetPackageType() == PackageType.PlayerDisconnect)
                {
                    int id = stream.ReadInt();
                    
                    if (_monoServerPlayer.ContainsKey(id))
                    {
                        var mono = _monoServerPlayer[id];
                        GameObject.Destroy(mono.gameObject);

                        _monoServerPlayer.Remove(id);
                    }
                }

                if (stream.GetPackageType() == PackageType.PlayerPosition)
                {
                    int id = stream.ReadInt();
                    Vector3 position = stream.ReadVector3();
                    
                    if (!_monoServerPlayer.ContainsKey(id))
                    {
                        var mono1 = MonoEntity.Constructor<MonoServerPlayer>(null);
                        _monoServerPlayer.Add(id, mono1);
                    }
                    
                    var mono = _monoServerPlayer[id];
                    mono.SetPosition(position);
                }
            }
        }
    }
}