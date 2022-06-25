using Core.Chunks;

namespace Core.World
{
    public interface IWorld
    { 
        public Chunk GetChunk(int x, int z);
        public void СhunkСreated(Chunk chunk);
    }
}