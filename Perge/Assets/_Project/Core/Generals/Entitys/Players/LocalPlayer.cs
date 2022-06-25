using Core.PackageUtils;
using Core.PackageUtils.Packages;
using ENet;

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

        public override void Tick(float dt)
        {
            SetPosition(_player.transform.position);
   
            using (BlockStream stream = new BlockStream(PackageType.PlayerStats))
            {
                new PackagePlayerStats(stream, this);
                _client.SendBytes(PacketFlags.None, stream.ToArray());
            }
        }
    }
}