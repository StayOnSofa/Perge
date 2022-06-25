
namespace Core.Biomes
{
    public class BiomeHills : Biome
    {
        private NoiseMapGenerator _noiseMapGenerator;
        public override void SetDefaults()
        {
            Border = 0.1f;
            Lerp = 0.5f;
            BiomeFrequency = 0.4f;
            HeightFrequency = 3.0f;
            GroundJump = 15;

            _noiseMapGenerator = new NoiseMapGenerator(Seed);
        }
    }
}
