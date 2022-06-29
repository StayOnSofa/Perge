namespace Core.Biomes
{
    public abstract class Biome
    {
        private static int _biomeCount = 0;
        protected FastNoiseLite NoiseGenerator;

        protected int Seed;
        protected float Border = 0.5f;
        protected float Lerp = 0.1f;
        protected float BiomeFrequency = 1f;
        protected float HeightFrequency = 1f;

        protected int GroundJump = 16;
        protected int BasicHeight = 80;

        public void Init(int basicSeed)
        {
            InitNoiseGenerator(basicSeed + _biomeCount);
            SetDefaults();

            _biomeCount++;
        }

        public virtual void InitNoiseGenerator(int seed)
        {
            Seed = seed;

            NoiseGenerator = new FastNoiseLite(seed);
            NoiseGenerator.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            NoiseGenerator.SetFractalType(FastNoiseLite.FractalType.FBm);
        }

        public abstract void SetDefaults();

        private float GenerateBiomeNoise(int x, int z)
        {
            return NoiseGenerator.GetNoise(x / BiomeFrequency, z / BiomeFrequency);
        }

        private float GenerateHeighNoise(int x, int z)
        {
            return NoiseGenerator.GetNoise(x / HeightFrequency, z / HeightFrequency);
        }

        public int GetHeight(int x, int z)
        {
            float height = GenerateHeighNoise(x, z);

            return (int)(height * GroundJump) + BasicHeight;
        }

        public float GetPower(int x, int z)
        {
            float devide = Lerp * 2f;
            float biome = (GenerateBiomeNoise(x,z) - (Border)) / devide;

            return biome;
        }

        public bool IsHere(int x, int z)
        {
            return GenerateBiomeNoise(x, z) > Border;
        }

        public abstract ushort GetBlock(int x, int height, int z, int topHeight);
    }
}