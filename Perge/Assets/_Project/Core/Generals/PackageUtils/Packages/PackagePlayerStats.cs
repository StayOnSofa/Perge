using UnityEngine;

namespace Core.PackageUtils.Packages
{
    public struct PackagePlayerStats
    {
        public Vector3 Position;

        public PackagePlayerStats(BlockStream stream)
        {
            Position = stream.ReadVector3();
        }

        public PackagePlayerStats(BlockStream stream, Player player)
        {
            Position = player.Position;
            stream.WriteVector3(Position);
        }
    }
}