namespace Core.Generals
{
    public class CoreRegister
    {
        public static ushort AIR;
        public static ushort DIRT;
        public static ushort GRASS;
        public static ushort STONE;
        public static ushort SAND;
        public static ushort STRUCTURE;
        public static ushort BIRCHLOG;
        public static ushort BIRCHLEAVES;
        
        [BlockRegister]
        public void Register()
        {
            AIR = BlockRegister.Register<Air>();   
            DIRT = BlockRegister.Register<Dirt>();   
            GRASS = BlockRegister.Register<Grass>();
            STONE = BlockRegister.Register<Stone>();
            SAND = BlockRegister.Register<Sand>();
            STRUCTURE = BlockRegister.Register<BlockStructure>();
            BIRCHLOG = BlockRegister.Register<BirchLog>();
            BIRCHLEAVES = BlockRegister.Register<BirchLeaves>();
        }
    }
}