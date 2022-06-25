using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Biomes
{
    public class NoiseMapGenerator
    {
        private const float c_MaxBiomeSize = 4f;

        private FastNoiseLite _biomeMap;
        private FastNoiseLite _biomeSmoothMap;
        private FastNoiseLite _heightMap;
        private FastNoiseLite _objectsOnGroundMap;

        private void InitBiomeMap()
        {
            _biomeMap = new FastNoiseLite();

            _biomeMap.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _biomeMap.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
            _biomeMap.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            _biomeMap.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            _biomeMap.SetDomainWarpAmp(20);

        }

        private void InitObjectOnGroundMap()
        {
            _objectsOnGroundMap = new FastNoiseLite();

            _objectsOnGroundMap.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _objectsOnGroundMap.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
            _objectsOnGroundMap.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
        }

        private void InitBiomeSmoothMap()
        {
            InitObjectOnGroundMap();
            _biomeSmoothMap = new FastNoiseLite();

            _biomeSmoothMap.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _biomeSmoothMap.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
            _biomeSmoothMap.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Div);
        }

        private void InitHeightMap()
        {
            _heightMap = new FastNoiseLite();
            _heightMap.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _heightMap.SetFractalType(FastNoiseLite.FractalType.FBm);
        }

        public NoiseMapGenerator(int seed)
        {
            InitBiomeMap();
            InitBiomeSmoothMap();
            InitHeightMap();

            _biomeMap.SetSeed(seed);
            _biomeSmoothMap.SetSeed(seed);
            _heightMap.SetSeed(seed);
        }

        public float GetHeight(int x, int y, float frequency)
        {
           return _heightMap.GetNoise(x / frequency, y / frequency);
        }

        public int GetBiome(int x, int y, int biomeCount)
        {
            float noise = _biomeMap.GetNoise(x/ c_MaxBiomeSize, y/ c_MaxBiomeSize);
            float value = (noise + 1f) / 2f;

            int biomeId = Mathf.RoundToInt(value * biomeCount);
            return biomeId;
        }

        public bool GetObjectSpawn(int x, int y, int chance)
        {
            float noise = _biomeMap.GetNoise(x * 512, y * 512);
            float value = (noise + 1f) / 2f;

            int valueId = Mathf.RoundToInt(value * chance);

            if (valueId == 0)
            {
                return true;
            }

            return false;
        }


        public float GetBiomeSmooth(int x, int y, float power)
        {
            float value = -_biomeSmoothMap.GetNoise(x/ c_MaxBiomeSize, y/ c_MaxBiomeSize);

            if (value < 0.01f)
            {
                value = 0;
            }

            value *= power;
            if (value > 1.0f)
            {
                value = 1f;
            }

            return value;
        }

    }
}
