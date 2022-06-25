namespace Core.Biomes
{
    public class BiomeHandler
    {
        private Biome _defaultBiome = new BiomeHills();

        public Biome GetPrimaryBiome(int x, int z)
        {
            return _defaultBiome;
        }

        public int GetMixedHeight(int x, int z)
        {
            float height = _defaultBiome.GetHeight(x, z);
            return (int)height;
        }
    }
}