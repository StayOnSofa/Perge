using System.Collections.Generic;
using UnityEngine;

namespace Core.Biomes
{
    public class BiomeHandler
    {
        private Biome _defaultBiome;
        private List<Biome> _biomes = new List<Biome>();
        
        public BiomeHandler()
        {
            _defaultBiome = new BiomeHills();

            _biomes.Add(new BiomeDesert());
            _biomes.Add(new BiomeMountain());

            InitBiomes(1337);
        }

        private void InitBiomes(int seed)
        {
            _defaultBiome.Init(seed);

            foreach (Biome biome in _biomes)
            {
                biome.Init(seed);
            }
        }

        public Biome GetPrimaryBiome(int x, int z)
        {
            foreach (Biome biome in _biomes)
            {
                if (biome.IsHere(x, z))
                {
                    return biome;
                }
            }

            return _defaultBiome;
        }

        public int GetMixedHeight(int x, int z)
        {
            float height = _defaultBiome.GetHeight(x, z);

            for (int i = (_biomes.Count - 1); i >= 0; i--)
            {
                Biome biome = _biomes[i];
                if (biome.IsHere(x, z))
                {
                    height = Mathf.Lerp(height, biome.GetHeight(x, z), biome.GetPower(x, z));
                }
            }

            return (int)height;
        }
    }
}