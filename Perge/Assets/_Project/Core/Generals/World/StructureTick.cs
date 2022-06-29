namespace Core.World
{
    public class StructureTick
    {
        public ushort BlockId;

        public int X;
        public int Y;
        public int Z;

        public StructureTick(int x, int y, int z, ushort blockId)
        {
            BlockId = blockId;

            X = x;
            Y = y;
            Z = z;
        }
    }
}