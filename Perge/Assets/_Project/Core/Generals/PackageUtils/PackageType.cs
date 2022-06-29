namespace Core.PackageUtils
{
    public enum PackageType : byte
    {
        PlayerStats = 0,
        ChunkUpload = 2,
        ChunkUnload = 3,
        
        PlayerDisconnect = 4,
        PlayerConnect = 5,
        PlayerPosition = 6,
        Empty = 0,
    }
}