using Core.Chunks;
using Core.PackageUtils;
using Core.PackageUtils.Packages;
using ENet;
using UnityEngine;

namespace Core
{
    public class LocalPlayer : Player
    {
        private DedicatedClient _client;
        private MonoPlayer _player;
        
        public LocalPlayer(DedicatedClient client) : base("LocalPlayer")
        {
            _client = client;
            _player = MonoEntity.Constructor<MonoPlayer>(this);
        }

        public MonoPlayer GetMonoPlayer()
        {
            return _player;
        }

        public override void Tick(float dt)
        {
            SetPosition(_player.transform.position);
   
            using (BlockStream stream = new BlockStream(PackageType.PlayerStats, false))
            {
                new PackagePlayerStats(stream, this);
                _client.SendBytes(PacketFlags.None, stream.ToArray());
            }
        }

        public Camera GetCamera()
        {
            return _player.GetCamera();
        }

        public KeyIndex GetChunkBorder()
        {
            var position = Position;
            return KeyIndex.ToChunkCoordinates((int)position.x, (int)position.z);
        }
    }
}