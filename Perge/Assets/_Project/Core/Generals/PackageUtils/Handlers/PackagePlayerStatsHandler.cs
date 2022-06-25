using Core.PackageUtils.Packages;
using UnityEngine;

namespace Core.PackageUtils.Handlers
{
    public class PackagePlayerStatsHandler : PackageServerHandler
    {
        public PackagePlayerStatsHandler(DedicatedServer server) : base(PackageType.PlayerStats, server)
        {
            return;
        }

        public override void Handle(Player player, BlockStream stream)
        {
            PackagePlayerStats stats = new PackagePlayerStats(stream);
            player.SetPosition(stats.Position);
        }
    }
}