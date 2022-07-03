
using Core.Generals;

namespace Core.Biomes
{
    public class BiomeHills : Biome
    {
        private NoiseMapGenerator _noiseMapGenerator;
        public override void SetDefaults()
        {
            Border = 0.2f;
            Lerp = 0.2f;
            BiomeFrequency = 4f;
            HeightFrequency = 1.2f;
            GroundJump = 7;

            _noiseMapGenerator = new NoiseMapGenerator(Seed);
        }

        public override void InitNoiseGenerator(int seed)
        {
            Seed = seed;

            NoiseGenerator = new FastNoiseLite(seed + 225);
            
            NoiseGenerator.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            NoiseGenerator.SetFractalType(FastNoiseLite.FractalType.PingPong);
        }
        
        public override ushort GetBlock(int x, int height, int z, int topHeight)
        {
            if (topHeight == height)
            {
                bool place = _noiseMapGenerator.GetObjectSpawn(x, z, 64);
                bool poppy = _noiseMapGenerator.GetObjectSpawn(x + 612, z + 212, 9);
                bool grass = _noiseMapGenerator.GetObjectSpawn(x + 521, z + 812, 6);
                
                if (place)
                {
                    return CoreRegister.TREEBIRCH;
                }

                if (poppy)
                {
                    return CoreRegister.POPPY;
                }

                if (grass)
                {
                    //return CoreRegister.SAND;
                }
            }
            else
            {
                ushort blockId = CoreRegister.GRASS;
                if (height < (topHeight - 6))
                {
                    blockId = CoreRegister.STONE;
                }

                return blockId;
            }

            return CoreRegister.AIR;
        }
    }
}
