using Core.Generals;

namespace Core.Biomes
{
    public class BiomeDesert : Biome
    {
        private NoiseMapGenerator _noiseMapGenerator;
        public override void SetDefaults()
        {
            Border = 0.3f;
            Lerp = 0.71f;
            BiomeFrequency = 16f;
            HeightFrequency = 1f;
            GroundJump = 2;
            BasicHeight = 65;

            _noiseMapGenerator = new NoiseMapGenerator(Seed + 102);
        }

        public override void InitNoiseGenerator(int seed)
        {
            Seed = seed;

            NoiseGenerator = new FastNoiseLite(seed + 22);
            
            NoiseGenerator.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            NoiseGenerator.SetFractalType(FastNoiseLite.FractalType.PingPong);
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