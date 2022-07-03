using Core.Generals;

namespace Core.Biomes
{
    public class BiomeMountain : Biome
    {
        private NoiseMapGenerator _noiseMapGenerator;
        public override void SetDefaults()
        {
            Border = 0.2f;
            Lerp = 0.05f;
            BiomeFrequency = 4f;
            HeightFrequency = 1.2f;
            GroundJump = 60;

            _noiseMapGenerator = new NoiseMapGenerator(Seed+2);
        }

        public override void InitNoiseGenerator(int seed)
        {
            Seed = seed;

            NoiseGenerator = new FastNoiseLite(seed);
            NoiseGenerator.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            NoiseGenerator.SetFractalType(FastNoiseLite.FractalType.PingPong);
        }

        public override ushort GetBlock(int x, int height, int z, int topHeight)
        {
            ushort blockId = CoreRegister.STONE;
            return blockId;
        }
    }
}