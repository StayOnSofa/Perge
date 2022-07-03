namespace Core.Generals
{
    public class CoreRegister
    {
        public static ushort AIR;
        public static ushort DIRT;
        public static ushort GRASS;
        public static ushort STONE;
        public static ushort SAND;
        public static ushort TREEBIRCH;
        public static ushort BIRCHLOG;
        public static ushort BIRCHLEAVES;
        public static ushort POPPY;
        public static ushort GRASSPLANT;
        
        [BlockRegister]
        public void Register()
        {
            AIR = BlockRegister.Register<Air>();   
            DIRT = BlockRegister.Register<Dirt>();   
            GRASS = BlockRegister.Register<Grass>();
            STONE = BlockRegister.Register<Stone>();
            SAND = BlockRegister.Register<Sand>();
            TREEBIRCH = BlockRegister.Register<BirchBlock>();
            BIRCHLOG = BlockRegister.Register<BirchLog>();
            BIRCHLEAVES = BlockRegister.Register<BirchLeaves>();
            POPPY = BlockRegister.Register<Poppy>();
            GRASSPLANT = BlockRegister.Register<GrassPlant>();
        }
    }
}