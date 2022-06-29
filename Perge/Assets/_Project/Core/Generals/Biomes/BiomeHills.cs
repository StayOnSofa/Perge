
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
            BiomeFrequency = 2f;
            HeightFrequency = 3f;
            GroundJump = 3;

            _noiseMapGenerator = new NoiseMapGenerator(Seed);
        }

        public override ushort GetBlock(int x, int height, int z, int topHeight)
        {
            if (topHeight == height)
            {
                bool place = _noiseMapGenerator.GetObjectSpawn(x, z, 64);
                bool poppy = _noiseMapGenerator.GetObjectSpawn(x + 612, z + 212, 8);

                if (place)
                {
                    return CoreRegister.STRUCTURE;
                }

                if (poppy)
                {
                    //return BlockRegister.Sphere;
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
