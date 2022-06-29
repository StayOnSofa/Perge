using Core.Generals;

namespace Core.Biomes
{
    public class BiomeDesert : Biome
    {
        private NoiseMapGenerator _noiseMapGenerator;
        public override void SetDefaults()
        {
            Border = 0.6f;
            Lerp = 0.71f;
            BiomeFrequency = 4f;
            HeightFrequency = 1f;
            GroundJump = 2;
            BasicHeight = 70;

            _noiseMapGenerator = new NoiseMapGenerator(Seed + 1);
        }

        public override ushort GetBlock(int x, int height, int z, int topHeight)
        {
            if (topHeight == height)
            {
                bool place = _noiseMapGenerator.GetObjectSpawn(x, z, 512);
                if (place)
                {
                    //return BlockRegister.StructureCactus;
                }
            }

            return CoreRegister.SAND;
        }
    }
}