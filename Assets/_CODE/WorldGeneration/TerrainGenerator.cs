using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

namespace _CODE.WorldGeneration
{
    [CreateAssetMenu(menuName = "World Generation/World Generation")]
    public class TerrainGenerator : ScriptableObject
    {
        [SerializeField] float scale =.2f;
        public float BaseHeight = 8;
        public NoiseOctaveSettings[] Octaves;
        public NoiseOctaveSettings DomainWarp;
        
        [Serializable]
        public class NoiseOctaveSettings
        {
            public FastNoiseLite.NoiseType NoiseType;
            public float Frequency = 0.2f;
            public float Amplitude = 1;
        }

        private FastNoiseLite[] octaveNoises;

        private FastNoiseLite warpNoise;
        
        static ProfilerMarker generationMarker = new ProfilerMarker(ProfilerCategory.Loading, "Generating");

        public void Init()
        {
            octaveNoises = new FastNoiseLite[Octaves.Length];
            for (int i = 0; i < Octaves.Length; i++)
            {
                octaveNoises[i] = new FastNoiseLite();
                octaveNoises[i].SetNoiseType(Octaves[i].NoiseType);
                octaveNoises[i].SetFrequency(Octaves[i].Frequency);
            }
            
            warpNoise = new FastNoiseLite();
            warpNoise.SetNoiseType(DomainWarp.NoiseType);
            warpNoise.SetFrequency(DomainWarp.Frequency);
            warpNoise.SetDomainWarpAmp(DomainWarp.Amplitude);
        }
        public BlockType[] GenerateCave(float offsetX, float offsetZ)
        {
            generationMarker.Begin();
            var result = new BlockType[MeshBuilder.ChunkWidth * MeshBuilder.ChunkHeight * MeshBuilder.ChunkWidth];
            for (int x = 0; x < MeshBuilder.ChunkWidth; x++)
            {
                for (int z = 0; z < MeshBuilder.ChunkWidth; z++)
                {
                    //float height  = Mathf.PerlinNoise((x/4f+offsetX) * scale, (z/4f+offsetZ) * scale) * 10 +15;
                    float height = GetHeight(x * MeshBuilder.BlockScale + offsetX,
                        z * MeshBuilder.BlockScale + offsetZ);
                    for (int y = 0; y < height /MeshBuilder.BlockScale; y++)
                    {
                        int index = x + y * MeshBuilder.ChunkWidthSQ + z * MeshBuilder.ChunkWidth;
                        result[index] = BlockType.Rock;
                    }
                }
            }

            generationMarker.End();
            return result;
        }

        public float GetHeight(float x, float y)
        {
            warpNoise.DomainWarp(ref x, ref y);
            
            float result = BaseHeight;

            for (int i = 0; i < Octaves.Length; i++)
            {
                float noise = octaveNoises[i].GetNoise(x, y);
                result += noise * Octaves[i].Amplitude / 2;
            }

            return result;
        }
        
        #region Noises
        
        public float perlinMax;
        public float perlinMin;
        public float twist = 1;
        public float turbulence = 1;
        
        public static float PerlinNoise3DAlt(float x, float y, float z)
        {
            y += 1;
            z += 2;
            float xy = _perlin3DFixed(x, y);
            float xz = _perlin3DFixed(x, z);
            float yz = _perlin3DFixed(y, z);
            float yx = _perlin3DFixed(y, x);
            float zx = _perlin3DFixed(z, x);
            float zy = _perlin3DFixed(z, y);
        
            return xy * xz * yz * yx * zx * zy;
        }
        
        static float _perlin3DFixed(float a, float b)
        {
            return Mathf.Sin(Mathf.PI * Mathf.PerlinNoise(a, b));
        }
         float PerlinNoise3D(float x, float y, float z)
        {
            float xy = Mathf.PerlinNoise(x, y);
            float yz = Mathf.PerlinNoise(y, z);
            float xz = Mathf.PerlinNoise(x, z);
            float xyz = xy * yz * xz;
            return xyz;
        }
        
         float SpaghettiPerlinNoise(float x, float y, float z)
        {
            float twistX = Mathf.Sin(y * twist) * turbulence;
            float twistY = Mathf.Cos(z * twist) * turbulence;
            float twistZ = Mathf.Sin(x * twist) * turbulence;
            return PerlinNoise3D(x + twistX, y + twistY, z + twistZ);
        }
        #endregion
    }
}