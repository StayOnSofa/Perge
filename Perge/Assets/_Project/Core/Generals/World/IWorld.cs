using Core.Chunks;

namespace Core.World
{
    public interface IWorld
    { 
        public const float c_TicksPerSecond = 1f / 20;
        public Chunk GetChunk(int x, int z);
        
        public void PlaceBlock(ushort blockID, int x, int y, int z);
        public void BreakBlock(int x, int y, int z);

    }
}